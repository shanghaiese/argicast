using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using System.Collections;

namespace Syngenta.AgriCast.Charting.Service
{
    public class NeuQuant:ColorQuantizer
    {
        public static int ncycles	=	100;			// no. of learning cycles

		public static int netsize  = 256;		// number of colours used
		public static int specials  = 3;		// number of reserved colours used
		public static int bgColour  = specials-1;	// reserved background colour
		public static int cutnetsize  = netsize - specials;
		public static int maxnetpos  = netsize-1;

		public static int initrad	 = netsize/8;   // for 256 cols, radius starts at 32
		public static int radiusbiasshift = 6;
		public static int radiusbias = 1 << radiusbiasshift;
		public static int initBiasRadius = initrad*radiusbias;
		public static int radiusdec = 30; // factor of 1/30 each cycle

		public static int alphabiasshift = 10;			// alpha starts at 1
		public static int initalpha      = 1<<alphabiasshift; // biased by 10 bits

		public static double gamma = 1024.0;
		public static double beta = 1.0/1024.0;
		public static double betagamma = beta * gamma;
    
		private double [][] network = new double [netsize][]; // the network itself
		protected int [][] colormap = new int [netsize][]; // the network itself
    
		private int [] netindex = new int [256]; // for network lookup - really 256
    
		private double [] bias = new double [netsize];  // bias and freq arrays for learning
		private double [] freq = new double [netsize];

		// four primes near 500 - assume no image has a length so large
		// that it is divisible by all four primes
   
		public static int prime1	=	499;
		public static int prime2	=	491;
		public static int prime3	=	487;
		public static int prime4	=	503;
		public static int maxprime	=	prime4;
    
		protected Color[] pixels = null;
		private int samplefac = 0;


		public NeuQuant (Bitmap im):this(1) {
			setPixels(im);
			setUpArrays ();
		}
    
		public NeuQuant (int sample, Bitmap im):this(sample) {
			setPixels(im);
			setUpArrays ();
		}
		public NeuQuant (int sample, Color[] cpixels):this(sample) {
			pixels = cpixels;
			setUpArrays ();
		}
		private NeuQuant (int sample)  {
			if (sample < 1) throw new IOException("Sample must be 1..30");
			if (sample > 30) throw new IOException("Sample must be 1..30");
			samplefac = sample;
			// rest later
		}
		public override int getColorCount () {
			return netsize;
		}

		public override Color getColor (int i) {
			if (i < 0 || i >= netsize) return Color.Transparent;
			int bb = colormap[i][0];
			int gg = colormap[i][1];
			int rr = colormap[i][2];
			return Color.FromArgb(rr, gg, bb);
		}
		protected void setUpArrays () {
			for (int x = 0; x < netsize; x++) {
				network[x] = new double[3];
				colormap[x] = new int[4];
			}
			network [0][0] = 0.0;	// black
			network [0][1] = 0.0;
			network [0][2] = 0.0;
    	
			network [1][0] = 1.0;	// white
			network [1][1] = 1.0;
			network [1][2] = 1.0;

			// RESERVED bgColour	// background
    	
			for (int i=0; i<specials; i++) {
				freq[i] = 1.0 / netsize;
				bias[i] = 0.0;
			}
        
			for (int i=specials; i<netsize; i++) {
				double [] p = network[i];
				p[0] = (256.0 * (i-specials)) / cutnetsize;
				p[1] = (256.0 * (i-specials)) / cutnetsize;
				p[2] = (256.0 * (i-specials)) / cutnetsize;

				freq[i] = 1.0 / netsize;
				bias[i] = 0.0;
			}
		}    	
    
		private void setPixels (Bitmap im) {
			int w = im.Width;
			int h = im.Height;
			int s = w*h;
			if (s < maxprime) throw new IOException ("Image is too small");
			pixels = new Color[s];
			int i = 0;
			for (int x = 0; x < w; x++) {
				for (int y = 0; y < h; y++) {
					pixels[i++] = im.GetPixel(x,y);
				}
			}
		}

		public override void init () {
			learn();
			fix();
			inxbuild();
		}
		public override int lookup (Color c) {
			return inxsearch(c.B, c.G, c.R);
		}

		private void altersingle(double alpha, int i, double b, double g, double r) {
			// Move neuron i towards biased (b,g,r) by factor alpha
			double [] n = network[i];				// alter hit neuron
			n[0] -= (alpha*(n[0] - b));
			n[1] -= (alpha*(n[1] - g));
			n[2] -= (alpha*(n[2] - r));
		}

		private void alterneigh(double alpha, int rad, int i, double b, double g, double r) {
        
			int lo = i-rad;   if (lo<specials-1) lo=specials-1;
			int hi = i+rad;   if (hi>netsize) hi=netsize;

			int j = i+1;
			int k = i-1;
			int q = 0;
			while ((j<hi) || (k>lo)) {
				double a = (alpha * (rad*rad - q*q)) / (rad*rad);
				q ++;
				if (j<hi) {
					double [] p = network[j];
					p[0] -= (a*(p[0] - b));
					p[1] -= (a*(p[1] - g));
					p[2] -= (a*(p[2] - r));
					j++;
				}
				if (k>lo) {
					double [] p = network[k];
					p[0] -= (a*(p[0] - b));
					p[1] -= (a*(p[1] - g));
					p[2] -= (a*(p[2] - r));
					k--;
				}
			}
		}
    
		private int contest (double b, double g, double r) {    // Search for biased BGR values
			// finds closest neuron (min dist) and updates freq 
			// finds best neuron (min dist-bias) and returns position 
			// for frequently chosen neurons, freq[i] is high and bias[i] is negative 
			// bias[i] = gamma*((1/netsize)-freq[i]) 

			double bestd = float.MaxValue;
			double bestbiasd = bestd;
			int bestpos = -1;
			int bestbiaspos = bestpos;
        
			for (int i=specials; i<netsize; i++) {
				double [] n = network[i];
				double dist = n[0] - b;   if (dist<0) dist = -dist;
				double a = n[1] - g;   if (a<0) a = -a;
				dist += a;
				a = n[2] - r;   if (a<0) a = -a;
				dist += a;
				if (dist<bestd) {bestd=dist; bestpos=i;}
				double biasdist = dist - bias [i];
				if (biasdist<bestbiasd) {bestbiasd=biasdist; bestbiaspos=i;}
				freq [i] -= beta * freq [i];
				bias [i] += betagamma * freq [i];
			}
			freq[bestpos] += beta;
			bias[bestpos] -= betagamma;
			return bestbiaspos;
		}
    
		private int specialFind (double b, double g, double r) {
			for (int i=0; i<specials; i++) {
				double [] n = network[i];
				if (n[0] == b && n[1] == g && n[2] == r) return i;
			}
			return -1;
		}
    
		private void learn() {
			int biasRadius = initBiasRadius;
			int alphadec = 30 + ((samplefac-1)/3);
			int lengthcount = pixels.Length;
			int samplepixels = lengthcount / samplefac;
			int delta = samplepixels / ncycles;
			int alpha = initalpha;

			int i = 0;
			int rad = biasRadius >> radiusbiasshift;
			if (rad <= 1) rad = 0;
	
//			Innobit.Fwk.Common.logDebug("beginning 1D learning: samplepixels=" + samplepixels + "  rad=" + rad);

			int step = 0;
			int pos = 0;
        
			if ((lengthcount%prime1) != 0) step = prime1;
			else {
				if ((lengthcount%prime2) !=0) step = prime2;
				else {
					if ((lengthcount%prime3) !=0) step = prime3;
					else step = prime4;
				}
			}
	
			i = 0;
			while (i < samplepixels) {
				Color p = pixels [pos];
				double b = p.B;
				double g = p.G;
				double r = p.R;

				if (i == 0) {   // remember background colour
					network [bgColour] [0] = b;
					network [bgColour] [1] = g;
					network [bgColour] [2] = r;
				}

				int j = specialFind (b, g, r);
				j = j < 0 ? contest (b, g, r) : j;

				if (j >= specials) {   // don't learn for specials
					double a = (1.0 * alpha) / initalpha;
					altersingle (a, j, b, g, r);
					if (rad > 0) alterneigh (a, rad, j, b, g, r);   // alter neighbours
				}

				pos += step;
				while (pos >= lengthcount) pos -= lengthcount;
	        
				i++;
				if (i%delta == 0) {	
					alpha -= alpha / alphadec;
					biasRadius -= biasRadius / radiusdec;
					rad = biasRadius >> radiusbiasshift;
					if (rad <= 1) rad = 0;
				}
			}
//			Innobit.Fwk.Common.logDebug("finished 1D learning: alpha=" + (1.0 * alpha)/initalpha + "!");
		}

		private void fix() {
			for (int i=0; i<netsize; i++) {
				for (int j=0; j<3; j++) {
					int x = (int) (0.5 + network[i][j]);
					if (x < 0) x = 0;
					if (x > 255) x = 255;
					colormap[i][j] = x;
				}
				colormap[i][3] = i;
			}
		}

		private void inxbuild() {
			// Insertion sort of network and building of netindex[0..255]

			int previouscol = 0;
			int startpos = 0;

			for (int i=0; i<netsize; i++) {
				int[] p = colormap[i];
				int[] q = null;
				int smallpos = i;
				int smallval = p[1];			// index on g
				// find smallest in i..netsize-1
				for (int j=i+1; j<netsize; j++) {
					q = colormap[j];
					if (q[1] < smallval) {		// index on g
						smallpos = j;
						smallval = q[1];	// index on g
					}
				}
				q = colormap[smallpos];
				// swap p (i) and q (smallpos) entries
				if (i != smallpos) {
					int j = q[0];   q[0] = p[0];   p[0] = j;
					j = q[1];   q[1] = p[1];   p[1] = j;
					j = q[2];   q[2] = p[2];   p[2] = j;
					j = q[3];   q[3] = p[3];   p[3] = j;
				}
				// smallval entry is now in position i
				if (smallval != previouscol) {
					netindex[previouscol] = (startpos+i)>>1;
					for (int j=previouscol+1; j<smallval; j++) netindex[j] = i;
					previouscol = smallval;
					startpos = i;
				}
			}
			netindex[previouscol] = (startpos+maxnetpos)>>1;
			for (int j=previouscol+1; j<256; j++) netindex[j] = maxnetpos; // really 256
		}
		protected int inxsearch(int b, int g, int r) {
			// Search for BGR values 0..255 and return colour index
			int bestd = 1000;		// biggest possible dist is 256*3
			int best = -1;
			int i = netindex[g];	// index on g
			int j = i-1;		// start at netindex[g] and work outwards

			while ((i<netsize) || (j>=0)) {
				if (i<netsize) {
					int [] p = colormap[i];
					int dist = p[1] - g;		// inx key
					if (dist >= bestd) i = netsize;	// stop iter
					else {
						if (dist<0) dist = -dist;
						int a = p[0] - b;   if (a<0) a = -a;
						dist += a;
						if (dist<bestd) {
							a = p[2] - r;   if (a<0) a = -a;
							dist += a;
							if (dist<bestd) {bestd=dist; best=i;}
						}
						i++;
					}
				}
				if (j>=0) {
					int [] p = colormap[j];
					int dist = g - p[1]; // inx key - reverse dif
					if (dist >= bestd) j = -1; // stop iter
					else {
						if (dist<0) dist = -dist;
						int a = p[0] - b;   if (a<0) a = -a;
						dist += a;
						if (dist<bestd) {
							a = p[2] - r;   if (a<0) a = -a;
							dist += a;
							if (dist<bestd) {bestd=dist; best=j;}
						}
						j--;
					}
				}
			}
			return best;
		}

    }
}