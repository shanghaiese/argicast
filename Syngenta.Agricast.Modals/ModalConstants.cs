using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syngenta.Agricast.Modals
{
    public class ModalConstants
    {

        #region ColorMaps and Restriction Letters for RuleSets

        #region RULESET : - SW_GroundSprayFungicide
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayFungicide_Day_Red = "#808080";
        public const string SW_GroundSprayFungicide_Day_Amber = "#FF0000";
        public const string SW_GroundSprayFungicide_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayFungicide_Night_Red = "#90EE90";
        public const string SW_GroundSprayFungicide_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayFungicide_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_GroundSprayFungicide_A5 = "R";
        public const string SW_GroundSprayFungicide_A4 = "A";
        public const string SW_GroundSprayFungicide_A3 = "A";
        public const string SW_GroundSprayFungicide_C4 = "D";
        public const string SW_GroundSprayFungicide_A2 = "F";
        public const string SW_GroundSprayFungicide_A1 = "F";
        public const string SW_GroundSprayFungicide_B2 = "U";
        public const string SW_GroundSprayFungicide_B3 = "V";
        public const string SW_GroundSprayFungicide_C3 = "D";
        public const string SW_GroundSprayFungicide_C1 = "D";
        public const string SW_GroundSprayFungicide_C2 = "T";
        public const string SW_GroundSprayFungicide_Default = "X";
        #endregion RULESET : - SW_GroundSprayFungicide

        #region RULESET : - SW_GroundSprayGeneric
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayGeneric_Day_Red = "#808080";
        public const string SW_GroundSprayGeneric_Day_Amber = "#FF0000";
        public const string SW_GroundSprayGeneric_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayGeneric_Night_Red = "#90EE90";
        public const string SW_GroundSprayGeneric_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayGeneric_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 
        public const string SW_GroundSprayGeneric_A5 = "R";
        public const string SW_GroundSprayGeneric_A4 = "A";
        public const string SW_GroundSprayGeneric_A3 = "A";
        public const string SW_GroundSprayGeneric_C4 = "D";
        public const string SW_GroundSprayGeneric_A2 = "F";
        public const string SW_GroundSprayGeneric_A1 = "F";
        public const string SW_GroundSprayGeneric_B2 = "U";
        public const string SW_GroundSprayGeneric_B3 = "V";
        public const string SW_GroundSprayGeneric_C3 = "D";
        public const string SW_GroundSprayGeneric_C1 = "D";
        public const string SW_GroundSprayGeneric_C2 = "T";
        public const string SW_GroundSprayGeneric_Default = "X";
        #endregion RULESET : - SW_GroundSprayGeneric

        #region RULESET : - SW_GroundSprayHerbicide - Pre Emergence
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayHerbicide_PE_Day_Red = "#808080";
        public const string SW_GroundSprayHerbicide_PE_Day_Amber = "#FF0000";
        public const string SW_GroundSprayHerbicide_PE_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_PE_Night_Red = "#90EE90";
        public const string SW_GroundSprayHerbicide_PE_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_PE_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_GroundSprayHerbicide_PE_A6 = "A";
        public const string SW_GroundSprayHerbicide_PE_A5 = "R";
        public const string SW_GroundSprayHerbicide_PE_A4 = "A";
        public const string SW_GroundSprayHerbicide_PE_A3 = "A";
        public const string SW_GroundSprayHerbicide_PE_C4 = "D";
        public const string SW_GroundSprayHerbicide_PE_A2 = "F";
        public const string SW_GroundSprayHerbicide_PE_A1 = "U";
        public const string SW_GroundSprayHerbicide_PE_B1 = "U";
        public const string SW_GroundSprayHerbicide_PE_B2 = "U";
        public const string SW_GroundSprayHerbicide_PE_B3 = "V";
        public const string SW_GroundSprayHerbicide_PE_C3 = "D";
        public const string SW_GroundSprayHerbicide_PE_C1 = "D";
        public const string SW_GroundSprayHerbicide_PE_C2 = "T";
        public const string SW_GroundSprayHerbicide_PE_Default = "X";
        #endregion RULESET : - SW_GroundSprayHerbicide - Pre Emergence

        #region RULESET : - SW_GroundSprayHerbicide - Small DropLets
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayHerbicide_SD_Day_Red = "#808080";
        public const string SW_GroundSprayHerbicide_SD_Day_Amber = "#FF0000";
        public const string SW_GroundSprayHerbicide_SD_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_SD_Night_Red = "#90EE90";
        public const string SW_GroundSprayHerbicide_SD_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_SD_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_GroundSprayHerbicide_SD_A5 = "R";
        public const string SW_GroundSprayHerbicide_SD_A4 = "A";
        public const string SW_GroundSprayHerbicide_SD_A3 = "A";
        public const string SW_GroundSprayHerbicide_SD_C4 = "D";
        public const string SW_GroundSprayHerbicide_SD_A2 = "F";
        public const string SW_GroundSprayHerbicide_SD_A1 = "U";
        public const string SW_GroundSprayHerbicide_SD_B1 = "U";
        public const string SW_GroundSprayHerbicide_SD_B2 = "U";
        public const string SW_GroundSprayHerbicide_SD_B3 = "V";
        public const string SW_GroundSprayHerbicide_SD_C3 = "D";
        public const string SW_GroundSprayHerbicide_SD_C1 = "D";
        public const string SW_GroundSprayHerbicide_SD_C2 = "T";
        public const string SW_GroundSprayHerbicide_SD_Default = "X";
        #endregion RULESET : - SW_GroundSprayHerbicide - Small DropLets

        #region RULESET : - SW_GroundSpray- Vine And Tree Crops
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayVineAndTree_Day_Red = "#808080";
        public const string SW_GroundSprayVineAndTree_Day_Amber = "#FF0000";
        public const string SW_GroundSprayVineAndTree_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayVineAndTree_Night_Red = "#90EE90";
        public const string SW_GroundSprayVineAndTree_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayVineAndTree_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_GroundSprayVineAndTree_A5 = "R";
        public const string SW_GroundSprayVineAndTree_A4 = "A";
        public const string SW_GroundSprayVineAndTree_A3 = "A";
        public const string SW_GroundSprayVineAndTree_C4 = "D";
        public const string SW_GroundSprayVineAndTree_A2 = "F";
        public const string SW_GroundSprayVineAndTree_A1 = "U";
        public const string SW_GroundSprayVineAndTree_B1 = "U";
        public const string SW_GroundSprayVineAndTree_B2 = "U";
        public const string SW_GroundSprayVineAndTree_B3 = "V";
        public const string SW_GroundSprayVineAndTree_C3 = "D";
        public const string SW_GroundSprayVineAndTree_C1 = "D";
        public const string SW_GroundSprayVineAndTree_C2 = "T";
        public const string SW_GroundSprayVineAndTree_Default = "X";
        #endregion RULESET : - SW_GroundSpray - Vine And Tree Crops

        #region RULESET : - SW_GroundSprayHerbicide - Large DropLets
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_GroundSprayHerbicide_LD_Day_Red = "#808080";
        public const string SW_GroundSprayHerbicide_LD_Day_Amber = "#FF0000";
        public const string SW_GroundSprayHerbicide_LD_Day_Green = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_LD_Night_Red = "#90EE90";
        public const string SW_GroundSprayHerbicide_LD_Night_Amber = "#D3D3D3";
        public const string SW_GroundSprayHerbicide_LD_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_GroundSprayHerbicide_LD_A5 = "R";
        public const string SW_GroundSprayHerbicide_LD_A4 = "A";
        public const string SW_GroundSprayHerbicide_LD_A3 = "A";
        public const string SW_GroundSprayHerbicide_LD_C4 = "D";
        public const string SW_GroundSprayHerbicide_LD_A2 = "F";
        public const string SW_GroundSprayHerbicide_LD_A1 = "U";
        public const string SW_GroundSprayHerbicide_LD_B1 = "U";
        public const string SW_GroundSprayHerbicide_LD_B2 = "U";
        public const string SW_GroundSprayHerbicide_LD_B3 = "V";
        public const string SW_GroundSprayHerbicide_LD_C3 = "D";
        public const string SW_GroundSprayHerbicide_LD_C1 = "D";
        public const string SW_GroundSprayHerbicide_LD_C2 = "T";
        public const string SW_GroundSprayHerbicide_LD_Default = "X";
        #endregion RULESET : - SW_GroundSprayHerbicide - Large DropLets


        #region RULESET : - SW_AerialSprayGeneric
        //Color Map for Spraying window RuleSet - Reference Color Map :- pal_spraywin
        public const string SW_AerialSprayGeneric_Day_Red = "#808080";
        public const string SW_AerialSprayGeneric_Day_Amber = "#FF0000";
        public const string SW_AerialSprayGeneric_Day_Green = "#D3D3D3";
        public const string SW_AerialSprayGeneric_Night_Red = "#90EE90";
        public const string SW_AerialSprayGeneric_Night_Amber = "#D3D3D3";
        public const string SW_AerialSprayGeneric_Night_Green = "#008000";


        //Constants to Letter for Rules in the RuleSet 

        public const string SW_AerialSprayGeneric_A5 = "R";
        public const string SW_AerialSprayGeneric_A4 = "A";
        public const string SW_AerialSprayGeneric_A3 = "A";
        public const string SW_AerialSprayGeneric_C4 = "D";
        public const string SW_AerialSprayGeneric_A2 = "F";
        public const string SW_AerialSprayGeneric_A1 = "F";
        public const string SW_AerialSprayGeneric_B2 = "U";
        public const string SW_AerialSprayGeneric_B3 = "V";
        public const string SW_AerialSprayGeneric_C3 = "D";
        public const string SW_AerialSprayGeneric_C1 = "D";
        public const string SW_AerialSprayGeneric_C2 = "T";
        public const string SW_AerialSprayGeneric_D1 = "D1";
        public const string SW_AerialSprayGeneric_D2 = "D2";
        public const string SW_AerialSprayGeneric_Default = "X";
        #endregion RULESET : - SW_AerialSprayGeneric

        #region RULESET :- PrecipType

        //Color Map for PrecipType :-  pal_precip_type
        public const string PrecipType_White = "#FFFFFF";
        public const string PrecipType_DarkBlue = "#000099";
        public const string PrecipType_LightBlue = "#0033FF";
        public const string PrecipType_LightGray = "#AAAAAA";
        public const string PrecipType_DarkGray = "#555555";


        //Define Constants 

        #endregion RULESET :- PrecipType

        #region RULESET :- DeltaT

        //Color Map for PrecipType :-  pal_precip_type
        public const string DeltaT_White = "#FFFFFF";
        public const string DeltaT_Yellow = "#fea98c";
        public const string DeltaT_Gray = "#d6d6d6";
        public const string DeltaT_Peach = "#fef561";

        //Define Constants 

        #endregion RULESET :- DeltaT

        #region RULESET :- FrostText
        public const string RS_FrostText_M50 = "#000066";
        public const string RS_FrostText_0 = "#B2B2FF";
        public const string RS_FrostText_0P1 = "#00FF77";
        public const string RS_FrostText_10 = "#FFFF00";
        public const string RS_FrostText_20 = "#FFCC00";
        public const string RS_FrostText_30 = "#FF8800";
        public const string RS_FrostText_40 = "#FF0000";
        public const string RS_FrostText_50 = "#880000";
        #endregion

        #region RuleSet :- RunOff
        public const string RS_RunOff_Red = "#FF0000";
        public const string RS_RunOff_Amber = "#FFA500";
        public const string RS_RunOff_White = "#FFFFFF";
        #endregion Runoff

        #endregion


    }
}
