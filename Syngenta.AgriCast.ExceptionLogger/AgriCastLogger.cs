using System;
using System.IO;
using Syngenta.AgriCast.ExceptionLogger;
using log4net;
using log4net.Appender;


namespace Syngenta.AgriCast.ExceptionLogger
{
    public class AgriCastLogger
    {

        #region Public properties
        /// <summary>
        /// This enum property is used for LogType.
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// Unknown type.
            /// </summary>
            Unknown,

            /// <summary>
            /// Info.
            /// </summary>
            Info,

            /// <summary>
            /// Error.
            /// </summary>
            Error,

            /// <summary>
            /// Debug.
            /// </summary>
            Debug,

            /// <summary>
            /// Warning.
            /// </summary>
            Warn,

            /// <summary>
            /// Fatal.
            /// </summary>
            Fatal
        }
        #endregion

        #region LoggerConstants used in this class

        protected static ILog log = null;
        //static int count = 0;
        //static bool IsSuccess = false;
        private const string ENCRYPTION_KEY = "252010104645AM";//"63200564514PM";
        static string connString;

        #endregion

        #region Configure log4net

        /// <summary>
        /// Check if log4net is configured
        /// </summary>
        public static bool IsConfigured
        {
            get
            {
                bool isConfigured = false;
                if (log != null && log.Logger.Repository.Configured)
                {
                    isConfigured = true;
                }
                return isConfigured;
            }
        }

        /// <summary>
        /// Configure the log4net
        /// </summary>
        public static void Configure()
        {
            Configure(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());
        }

        /// <summary>
        /// Configure the log4net
        /// </summary>
        public static void Configure(string logger)
        {
            Configure(logger, null);
        }


        /// <summary>
        /// Configure the log4net
        /// </summary>
        ///  <example>
        /// An example of a logger name = MyApplication
        /// /// </example>
        public static void Configure(string logger, string configFile)
        {
            //Gets the logger object
            log = LogManager.GetLogger(logger);

            if (configFile != null && configFile.Length != 0)
            {
                FileInfo fInfo = null;
                try
                {
                    fInfo = new FileInfo(System.Web.HttpRuntime.AppDomainAppPath + @"Bin\" + configFile);
                    //fInfo = new FileInfo(@"D:\Code\Syngenta.KAMDB\Syngenta.KAMDB.WebUILayer"+ @"\bin\" + configFile);
                    //commented by arun george... temp value.
                }
                catch
                {
                    fInfo = new FileInfo(@".\" + configFile);
                }

                //Configure the log4net by reading config file
                log4net.Config.XmlConfigurator.Configure(fInfo);
            }
            else
            {
                log4net.Config.XmlConfigurator.Configure();
            }

            //if there is AdoNetAppender, decrypt the connection
            //string and reassign it.
            log4net.Appender.IAppender[] app = log.Logger.Repository.GetAppenders();
            foreach (log4net.Appender.IAppender iAppender in app)
            {
                if (iAppender.GetType().Name == "AdoNetAppender")
                {
                    AdoNetAppender adoNetAppender = (AdoNetAppender)iAppender;
                    //create the encryption manager object and pass the key in the constructor
                    //SynEncryptionManager encryptionManager = new SynEncryptionManager(ENCRYPTION_KEY);
                    ////Use strategies which is used to encrypt the string
                    //encryptionManager.CryptographyStrategy = new DesCryptography();
                    ////set the decrypted value which has to be decrypted
                    //encryptionManager.EncryptedValue = adoNetAppender.ConnectionString;
                    ////Call decryptor 
                    //adoNetAppender.ConnectionString = encryptionManager.CreateDecryptor();
                    //refresh the appender
                    adoNetAppender.ActivateOptions();
                    break;
                }
            }
        }

        #endregion


        /// <summary>
        /// Publishes the exception depending on the Web.Config settings
        /// </summary>
        /// <param name="inSyncException"></param>
        /// <remarks>
        /// <para>
        /// The <c>EventID</c> of the event log entry can be
        /// set using the <c>EventLogEventID</c> property (<see cref="LoggingEvent.Properties"/>)
        /// on the <see cref="LoggingEvent"/>.
        /// </para>
        /// <para>
        /// There is a limit of 32K characters for an event log message
        /// </para>
        /// <para>
        /// When configuring the EventLogAppender a mapping can be
        /// specified to map a logging level to an event log entry type. For example:
        /// </para>
        /// <code lang="XML">
        /// &lt;mapping&gt;
        /// 	&lt;level value="ERROR" /&gt;
        /// 	&lt;eventLogEntryType value="Error" /&gt;
        /// &lt;/mapping&gt;
        /// &lt;mapping&gt;
        /// 	&lt;level value="DEBUG" /&gt;
        /// 	&lt;eventLogEntryType value="Information" /&gt;
        /// &lt;/mapping&gt;
        /// </code>
        /// <para>
        /// The Level is the standard log4net logging level and eventLogEntryType can be any value
        /// from the <see cref="EventLogEntryType"/> enum, i.e.:
        /// <list type="bullet">
        /// <item><term>Error</term><description>an error event</description></item>
        /// <item><term>Warning</term><description>a warning event</description></item>
        /// <item><term>Information</term><description>an informational event</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Logging events are sent to the file specified by
        /// the <see cref="File"/> property.
        /// </para>
        /// <para>
        /// The file can be opened in either append or overwrite mode 
        /// by specifying the <see cref="AppendToFile"/> property.
        /// If the file path is relative it is taken as relative from 
        /// the application base directory. The file encoding can be
        /// specified by setting the <see cref="Encoding"/> property.
        /// </para>
        /// <para>
        /// The layout's <see cref="ILayout.Header"/> and <see cref="ILayout.Footer"/>
        /// values will be written each time the file is opened and closed
        /// respectively. If the <see cref="AppendToFile"/> property is <see langword="true"/>
        /// then the file may contain multiple copies of the header and footer.
        /// </para>
        /// <para>
        /// This appender will first try to open the file for writing when <see cref="ActivateOptions"/>
        /// is called. This will typically be during configuration.
        /// If the file cannot be opened for writing the appender will attempt
        /// to open the file again each time a message is logged to the appender.
        /// If the file cannot be opened for writing when a message is logged then
        /// the message will be discarded by this appender.
        /// </para>
        /// <para>
        /// The <see cref="FileAppender"/> supports pluggable file locking models via
        /// the <see cref="LockingModel"/> property.
        /// The default behavior, implemented by <see cref="FileAppender.ExclusiveLock"/> 
        /// is to obtain an exclusive write lock on the file until this appender is closed.
        /// The alternative model, <see cref="FileAppender.MinimalLock"/>, only holds a
        /// write lock while the appender is writing a logging event.
        /// </para>
        /// <para>
        /// RollingFileAppender can roll log files based on size or date or both
        /// depending on the setting of the <see cref="RollingStyle"/> property.
        /// When set to <see cref="RollingMode.Size"/> the log file will be rolled
        /// once its size exceeds the <see cref="MaximumFileSize"/>.
        /// When set to <see cref="RollingMode.Date"/> the log file will be rolled
        /// once the date boundary specified in the <see cref="DatePattern"/> property
        /// is crossed.
        /// When set to <see cref="RollingMode.Composite"/> the log file will be
        /// rolled once the date boundary specified in the <see cref="DatePattern"/> property
        /// is crossed, but within a date boundary the file will also be rolled
        /// once its size exceeds the <see cref="MaximumFileSize"/>.
        /// When set to <see cref="RollingMode.Once"/> the log file will be rolled when
        /// the appender is configured. This effectively means that the log file can be
        /// rolled once per program execution.
        /// </para>
        /// <para>
        /// A of few additional optional features have been added:
        /// <list type="bullet">
        /// <item>Attach date pattern for current log file <see cref="StaticLogFileName"/></item>
        /// <item>Backup number increments for newer files <see cref="CountDirection"/></item>
        /// <item>Infinite number of backups by file size <see cref="MaxSizeRollBackups"/></item>
        /// </list>
        /// </para>
        /// 
        /// <note>
        /// <para>
        /// For large or infinite numbers of backup files a <see cref="CountDirection"/> 
        /// greater than zero is highly recommended, otherwise all the backup files need
        /// to be renamed each time a new backup is created.
        /// </para>
        /// <para>
        /// When Date/Time based rolling is used setting <see cref="StaticLogFileName"/> 
        /// to <see langword="true"/> will reduce the number of file renamings to few or none.
        /// </para>
        /// </note>
        /// 
        /// <note type="caution">
        /// <para>
        /// Changing <see cref="StaticLogFileName"/> or <see cref="CountDirection"/> without clearing
        /// the log file directory of backup files will cause unexpected and unwanted side effects.  
        /// </para>
        /// </note>
        /// 
        /// <para>
        /// If Date/Time based rolling is enabled this appender will attempt to roll existing files
        /// in the directory without a Date/Time tag based on the last write date of the base log file.
        /// The appender only rolls the log file when a message is logged. If Date/Time based rolling 
        /// is enabled then the appender will not roll the log file at the Date/Time boundary but
        /// at the point when the next message is logged after the boundary has been crossed.
        /// </para>
        /// 
        /// <para>
        /// The <see cref="RollingFileAppender"/> extends the <see cref="FileAppender"/> and
        /// has the same behavior when opening the log file.
        /// The appender will first try to open the file for writing when <see cref="ActivateOptions"/>
        /// is called. This will typically be during configuration.
        /// If the file cannot be opened for writing the appender will attempt
        /// to open the file again each time a message is logged to the appender.
        /// If the file cannot be opened for writing when a message is logged then
        /// the message will be discarded by this appender.
        /// </para>
        /// <para>
        /// When rolling a backup file necessitates deleting an older backup file the
        /// file to be deleted is moved to a temporary name before being deleted.
        /// </para>
        /// 
        /// <note type="caution">
        /// <para>
        /// A maximum number of backup files when rolling on date/time boundaries is not supported.
        /// </para>
        /// </note>
        /// <para>
        /// <see cref="AdoNetAppender"/> appends logging events to a table within a
        /// database. The appender can be configured to specify the connection 
        /// string by setting the <see cref="ConnectionString"/> property. 
        /// The connection type (provider) can be specified by setting the <see cref="ConnectionType"/>
        /// property. For more information on database connection strings for
        /// your specific database see <a href="http://www.connectionstrings.com/">http://www.connectionstrings.com/</a>.
        /// </para>
        /// <para>
        /// Records are written into the database either using a prepared
        /// statement or a stored procedure. The <see cref="CommandType"/> property
        /// is set to <see cref="System.Data.CommandType.Text"/> (<c>System.Data.CommandType.Text</c>) to specify a prepared statement
        /// or to <see cref="System.Data.CommandType.StoredProcedure"/> (<c>System.Data.CommandType.StoredProcedure</c>) to specify a stored
        /// procedure.
        /// </para>
        /// <para>
        /// The prepared statement text or the name of the stored procedure
        /// must be set in the <see cref="CommandText"/> property.
        /// </para>
        /// <para>
        /// The prepared statement or stored procedure can take a number
        /// of parameters. Parameters are added using the <see cref="AddParameter"/>
        /// method. This adds a single <see cref="AdoNetAppenderParameter"/> to the
        /// ordered list of parameters. The <see cref="AdoNetAppenderParameter"/>
        /// type may be subclassed if required to provide database specific
        /// functionality. The <see cref="AdoNetAppenderParameter"/> specifies
        /// the parameter name, database type, size, and how the value should
        /// be generated using a <see cref="ILayout"/>.
        /// </para>
        /// </remarks>
        /// <example>
        /// An example of a SQL Server table that could be logged to:
        /// <code lang="SQL">
        /// CREATE TABLE [dbo].[Log] ( 
        ///   [ID] [int] IDENTITY (1, 1) NOT NULL ,
        ///   [Date] [datetime] NOT NULL ,
        ///   [Thread] [varchar] (255) NOT NULL ,
        ///   [Level] [varchar] (20) NOT NULL ,
        ///   [Logger] [varchar] (255) NOT NULL ,
        ///   [Message] [varchar] (4000) NOT NULL 
        /// ) ON [PRIMARY]
        /// </code>
        /// </example>
        /// <example>
        /// An example configuration to log to the above table:
        /// <code lang="XML" escaped="true">
        /// <appender name="AdoNetAppender_SqlServer" type="log4net.Appender.AdoNetAppender" >
        ///   <connectionType value="System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" />
        ///   <connectionString value="data source=SQLSVR;initial catalog=test_log4net;integrated security=false;persist security info=True;User ID=sa;Password=sa" />
        ///   <commandText value="INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message]) VALUES (@log_date, @thread, @log_level, @logger, @message)" />
        ///   <parameter>
        ///     <parameterName value="@log_date" />
        ///     <dbType value="DateTime" />
        ///     <layout type="log4net.Layout.PatternLayout" value="%date{yyyy'-'MM'-'dd HH':'mm':'ss'.'fff}" />
        ///   </parameter>
        ///   <parameter>
        ///     <parameterName value="@thread" />
        ///     <dbType value="String" />
        ///     <size value="255" />
        ///     <layout type="log4net.Layout.PatternLayout" value="%thread" />
        ///   </parameter>
        ///   <parameter>
        ///     <parameterName value="@log_level" />
        ///     <dbType value="String" />
        ///     <size value="50" />
        ///     <layout type="log4net.Layout.PatternLayout" value="%level" />
        ///   </parameter>
        ///   <parameter>
        ///     <parameterName value="@logger" />
        ///     <dbType value="String" />
        ///     <size value="255" />
        ///     <layout type="log4net.Layout.PatternLayout" value="%logger" />
        ///   </parameter>
        ///   <parameter>
        ///     <parameterName value="@message" />
        ///     <dbType value="String" />
        ///     <size value="4000" />
        ///     <layout type="log4net.Layout.PatternLayout" value="%message" />
        ///   </parameter>
        /// </appender>
        /// </code>
        /// </example>
        public static void Publish(AgriCastException AgriCastException, LogType type)
        {
            try
            {
                log4net.GlobalContext.Properties["UserId"] = AgriCastException.UserId;
                log4net.GlobalContext.Properties["Service"] = AgriCastException.Service;
                log4net.GlobalContext.Properties["Module"] = AgriCastException.Module;
                log4net.GlobalContext.Properties["Server"] = AgriCastException.Server;
                log4net.GlobalContext.Properties["Domain"] = AgriCastException.Domain;
                log4net.GlobalContext.Properties["ExceptionType"] = AgriCastException.ExceptionType;
                log4net.GlobalContext.Properties["ExceptionDetail"] = AgriCastException.ExceptionDetail;

                if (AgriCastException.InnerException.GetType().FullName == "System.Threading.ThreadAbortException")
                    return;
             
                switch (type)
                {
                    case LogType.Debug:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Fatal:
                        if (log.IsFatalEnabled)
                        {
                            log.Fatal(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Info:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Warn:
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    default:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(AgriCastException.Message, AgriCastException);
                        }
                        break;
                }
            }
            catch (Exception)
            {
            } 
        }

        public static void Publish(AgriCastException AgriCastException, LogType type, string Audit)
        {
            try
            {
                log4net.GlobalContext.Properties["userIP"] = AgriCastException.UserIP;
                log4net.GlobalContext.Properties["userId"] = AgriCastException.UserId;
                log4net.GlobalContext.Properties["token"] = AgriCastException.Token;
                log4net.GlobalContext.Properties["culture"] = AgriCastException.Culture;
                log4net.GlobalContext.Properties["sessionID"] = AgriCastException.SessionID;
                log4net.GlobalContext.Properties["service"] = AgriCastException.Service;
                log4net.GlobalContext.Properties["module"] = AgriCastException.Module;
                log4net.GlobalContext.Properties["locSearchType"] = AgriCastException.LocSearchType;
                log4net.GlobalContext.Properties["locSearchString"] = AgriCastException.LocSearchString;
                log4net.GlobalContext.Properties["locSearchDatasource"] = AgriCastException.LocSearchDatasource;
                log4net.GlobalContext.Properties["numOfLocs"] = AgriCastException.NumOfLocs;
                log4net.GlobalContext.Properties["searchLatLong"] = AgriCastException.SearchLatLong;
                log4net.GlobalContext.Properties["countryName"] = AgriCastException.CountryName;
                log4net.GlobalContext.Properties["locName"] = AgriCastException.LocName;
                log4net.GlobalContext.Properties["weatherDatasource"] = AgriCastException.WeatherDatasource;
                log4net.GlobalContext.Properties["weatherLatLong"] = AgriCastException.WeatherLatLong;
                log4net.GlobalContext.Properties["weatherDateRange"] = AgriCastException.WeatherDateRange;
                log4net.GlobalContext.Properties["weatherSeries"] = AgriCastException.WeatherSeries;
                log4net.GlobalContext.Properties["weatherParams"] = AgriCastException.WeatherParams;

                if (AgriCastException.InnerException.GetType().FullName == "System.Threading.ThreadAbortException")
                    return;

                switch (type)
                {
                    case LogType.Debug:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Fatal:
                        if (log.IsFatalEnabled)
                        {
                            log.Fatal(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Info:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    case LogType.Warn:
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(AgriCastException.Message, AgriCastException);
                        }
                        break;
                    default:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(AgriCastException.Message, AgriCastException);
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// Publishes the exception depending on the
        /// Web.Config settings
        /// </summary>
        /// <param name="inSyncException"></param>
        public static void Publish(string message, LogType type)
        {
            try
            {
                switch (type)
                {
                    case LogType.Debug:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(message);
                        }
                        break;
                    case LogType.Fatal:
                        if (log.IsFatalEnabled)
                        {
                            log.Fatal(message);
                        }
                        break;
                    case LogType.Info:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(message);
                        }
                        break;
                    case LogType.Warn:
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(message);
                        }
                        break;
                    default:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(message);
                        }
                        break;
                }
            }
            catch (Exception)
            {
            }
        }
        /*
        #region GetServerSetting
        /// <summary>
        /// Gets the Values from the Web.Config
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        private static string GetServerSetting(string appKey)
        {
            System.Collections.IDictionary dictionary = (System.Collections.IDictionary)System.Configuration.ConfigurationSettings.GetConfig(LoggerConstants.LOCALHOST);
            return (string)dictionary[appKey];
        }
        #endregion

        #region GetConn()
        /// <summary>
        /// This method is used to get the connection string from the DL Layer
        /// </summary>
        /// <returns></returns>
        private static string GetConn()
        {
            if (connString == null)
            {
                #region Commented
                //// get Datasource without userid and pwd
                //string dataSource = GetServerSetting(LoggerConstants.DATASOURCE).ToString(); 
                ////get the Encription Key
                //SynEncryptionManager encryptionManager = new SynEncryptionManager(LoggerConstants.ENCRYPTION_KEY); 
                //encryptionManager.CryptographyStrategy = new DesCryptography();

                //// Get the excrypted User ID from Web.Config
                //string tempUserId = GetServerSetting(LoggerConstants.USER_ID);
                //encryptionManager.EncryptedValue = tempUserId;
                //// Decrypt the User ID
                //string userId = encryptionManager.CreateDecryptor();
                //dataSource = dataSource.Replace(LoggerConstants.USER_ID, userId);

                //// Get the excrypted Password from Web.Config
                //string tempPassword = GetServerSetting(LoggerConstants.PASSWORD);
                //encryptionManager.EncryptedValue = tempPassword;
                //// Decrypt the Password
                //string password = encryptionManager.CreateDecryptor();
                //dataSource = dataSource.Replace(LoggerConstants.PASSWORD, password);
                //connString = dataSource;
                #endregion Commented

                string connStr = (string)new System.Configuration.AppSettingsReader().GetValue("ConnectionString", typeof(string));

                SynEncryptionManager encryptionManager = new SynEncryptionManager(LoggerConstants.ENCRYPTION_KEY);
                encryptionManager.CryptographyStrategy = new DesCryptography();
                encryptionManager.EncryptedValue = (string)new System.Configuration.AppSettingsReader().GetValue("UserId", typeof(string));

                connStr = connStr.Replace("@UserId", (encryptionManager.CreateDecryptor()));

                encryptionManager.EncryptedValue = (string)new System.Configuration.AppSettingsReader().GetValue("Password", typeof(string));
                connStr = connStr.Replace("@Password", (encryptionManager.CreateDecryptor()));

                connString = connStr;
            }

            return connString;

            //strConnString;// DataAccessHelper.ConnectionString;// "Data Source=Sales1D_Synopsis; User Id=synopsis; Password=synopsis;connection lifetime=10;Min Pool Size=1;Max Pool Size=10;";

        }
        #endregion

        #region Configure
        /// <summary>
        /// This methods is used to configure the log4net using XML File
        /// </summary>
        /// <param name="logger">Logger Name</param>
        /// <param name="configFile"> xml file</param>
        /// <param name="appender">type of Appender</param>       
        private static void Configure(string logger, string appender)
        {
            //Gets the logger object
            log = LogManager.GetLogger(logger);
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.Load(LoggerConstants.LOG4NET_XML);
                // doc.Load(System.Configuration.ConfigurationSettings.AppSettings[LoggerConstants.LOG4NET_XML]);
                XmlNodeList nodeListAppender = doc.SelectNodes(LoggerConstants.APPENDER_NODE);
                for (int iCount = 0; iCount < nodeListAppender.Count; iCount++)
                {
                    XmlNode node = nodeListAppender.Item(iCount);
                    node.Attributes[LoggerConstants.APPENDER_ATTRIBUTE_REF].Value = appender;
                }

            }
            catch (Exception ex)
            {
                //fInfo = new FileInfo(@".\" + configFile);
            }
            //XmlNodeList nl = (XmlElement)(XmlNodeList)doc.GetElementsByTagName("log4net").Item(0);
            XmlElement element = (XmlElement)(XmlNode)doc.GetElementsByTagName(LoggerConstants.LOG4NET_ELEMENT).Item(0);
            //Configure the log4net by reading config file
            // log4net.Config.XmlConfigurator.Configure(fInfo);
            log4net.Config.XmlConfigurator.Configure(element);

        }
        #endregion

        #region Publish
        /// <summary>
        /// This method is used to Publish the exception
        /// </summary>
        /// <param name="kamException"> Object of KAMExcerption class</param>
        /// <param name="type">LogType</param>
        public static void Publish(KAMException kamException, LogType type)
        {
            try
            {
                //Avoid logging Thread Aborted exception in teh database
                if (kamException.InnerException.GetType().FullName == LoggerConstants.THREAD_ABORTED)
                    return;

                // Customizing the params 
                //log4net.MDC.Set(LoggerConstants.PARAM_PAGE_URL, kamException.ExceptionPath);
                //log4net.MDC.Set(LoggerConstants.PARAM_DISPLAY_MESSAGE, GetDisplayMessage(kamException));
                //log4net.MDC.Set(LoggerConstants.PARAM_METHOD_NAME, LoggerConstants.STRING_EMPTY);
                //log4net.MDC.Set(LoggerConstants.PARAM_ACUTAL_EXCEPTION, kamException.InnerException.ToString());
                //log4net.MDC.Set(LoggerConstants.PARAM_ISLOG, LoggerConstants.KAM_EX_TYPE);

                if (!IsSuccess)
                {
                    // Call the method Configure to configure the Log4net
                    //KAMLogger.Configure(HttpContext.Current.User.Identity.Name, LoggerConstants.APPENDERS[count]);
                   // KAMLogger.Configure("KAMDB", LoggerConstants.APPENDERS[count]);
                    // change this too...temp value put by Arun George

                }

                // Get all available appenders from the Repository
                log4net.Appender.IAppender[] appenderList = log.Logger.Repository.GetAppenders();

                // loop thru each appender and log the exception at the appropriate place
                for (int iCount = 0; iCount < appenderList.Length; iCount++)
                {
                    //foreach (log4net.Appender.IAppender iAppender in app)
                    //{
                    // increase the local variable values to get the next appender
                    if (count < 4)
                    {
                        count++;
                    }
                    log4net.Appender.IAppender appender = (log4net.Appender.IAppender)appenderList.GetValue(iCount);
                    //Switch case for various appenders
                    switch (appender.GetType().Name)
                    {
                        case LoggerConstants.ADO_NET_APPENDER:
                            try
                            {
                                AdoNetAppender adoNetAppender = (AdoNetAppender)appender;
                                adoNetAppender.ConnectionString = GetConn();
                                adoNetAppender.ActivateOptions();
                                // Call the ExceptionLogging method to log the exception
                                IsSuccess = ExceptionLogging(kamException, type, appender);
                                //IsSuccess = false;
                            }
                            catch
                            {
                                IsSuccess = false;
                            }
                            break;
                        case LoggerConstants.FILE_APPENDER:
                            if (!IsSuccess)
                            {
                                // Call the ExceptionLogging method to log the exception
                                IsSuccess = ExceptionLogging(kamException, type, appender);
                                //IsSuccess = true;
                            }
                            break;
                        case LoggerConstants.SMTP_APPENDER:
                            if (!IsSuccess)
                            {
                                // Call the ExceptionLogging method to log the exception
                                IsSuccess = ExceptionLogging(kamException, type, appender);
                                //IsSuccess = false;
                            }
                            break;
                        case LoggerConstants.EVENT_LOG_APPENDER:
                            if (!IsSuccess)
                            {
                                // Call the ExceptionLogging method to log the exception
                                IsSuccess = ExceptionLogging(kamException, type, appender);
                                //IsSuccess = false;
                            }
                            break;
                    }
                    // Exit the loop if exception is been logged successfully
                    if (IsSuccess)
                    {
                        break;
                    }
                    else
                    {
                        Publish(kamException, type);
                    }
                }

                IsSuccess = false;
                count = 0;
            }
            catch
            {
                IsSuccess = false;
                count = 0;
            }
        }
        #endregion

        #region ExceptionLogging
        /// <summary>
        /// This methos is used for logging an exception at the various locations
        /// </summary>
        /// <param name="kamException">Object for KAMException class</param>
        /// <param name="type">LogType</param>
        /// <param name="iAppender">Appender</param>
        /// <returns>Returns the boolean values</returns>
        private static bool ExceptionLogging(KAMException kamException, LogType type, log4net.Appender.IAppender appender)
        {
            try
            {
                switch (type)
                {
                    case LogType.Debug:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(kamException.Message, kamException);
                        }
                        break;
                    case LogType.Info:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(kamException.Message, kamException);
                        }
                        break;
                    case LogType.Warn:
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(kamException.Message, kamException);
                        }
                        break;
                    case LogType.Error:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(kamException.Message, kamException);
                        }
                        break;
                    case LogType.Fatal:
                        if (log.IsFatalEnabled)
                        {
                            log.Fatal(kamException.Message, kamException);
                        }
                        break;
                    default:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(kamException.Message, kamException);
                        }
                        break;
                }

                // Check for any error occored while logging an exception
                if (appender.CustomError != null && appender.CustomError.ToString().Length > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion ExceptionLogging

        #region KAMLog
        /// <summary>
        /// This is used for Logging mechanism
        /// </summary>
        /// <param name="strPageURL">User of the page</param>
        /// <param name="strLocation">Logger Location</param>
        /// <param name="strDisplayMsg">Messaged which user wants to log in the database</param>
        /// <param name="strLoggedUser">Logged in user</param>
        /// <param name="type">LogType</param>
        public static void KAMLog(string pageURL, string className, string methodName, LogType type, string displayMsg)
        {
            try
            {
                // Customize the parameters
                log4net.MDC.Set(LoggerConstants.PARAM_PAGE_URL, pageURL);
                log4net.MDC.Set(LoggerConstants.PARAM_DISPLAY_MESSAGE, "");
                log4net.MDC.Set(LoggerConstants.PARAM_METHOD_NAME, className + LoggerConstants.SPECIAL_DOT + methodName);
                log4net.MDC.Set(LoggerConstants.PARAM_ACUTAL_EXCEPTION, displayMsg);
                log4net.MDC.Set(LoggerConstants.PARAM_ISLOG, LoggerConstants.KAM_LOG_TYPE);


                //Configure the Log4Net
                //KAMLogger.Configure(HttpContext.Current.User.Identity.Name, LoggerConstants.APPENDERS[count]);
                KAMLogger.Configure("KAMDB", LoggerConstants.APPENDERS[count]);
                // change this too...temp value put by Arun George


                //Get the list of available Appender 
                log4net.Appender.IAppender appender = (log4net.Appender.IAppender)log.Logger.Repository.GetAppenders().GetValue(0);//(log4net.Appender.IAppender)app.GetValue(intCount);
                if (appender.GetType().Name == LoggerConstants.ADO_NET_APPENDER)
                {
                    AdoNetAppender adoNetAppender = (AdoNetAppender)appender;
                    adoNetAppender.ConnectionString = GetConn();
                    adoNetAppender.ActivateOptions();
                    // Call KAMLogging method to log the comment
                    KAMLogging(className, type);
                    //IsSuccess = false;
                }
                IsSuccess = false;
                count = 0;
            }
            catch
            {
                IsSuccess = false;
                count = 0;
            }
        }
        #endregion

        #region ExceptionLogging
        /// <summary>
        /// This method is used to log the comments for KAM
        /// </summary>
        /// <param name="strMsg">Message which user wants to log</param>
        /// <param name="type">LogType</param>
        private static void KAMLogging(string msg, LogType type)
        {
            try
            {
                switch (type)
                {
                    case LogType.Debug:
                        if (log.IsDebugEnabled)
                        {
                            log.Debug(msg);
                        }
                        break;
                    case LogType.Info:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(msg);
                        }
                        break;
                    case LogType.Warn:
                        if (log.IsWarnEnabled)
                        {
                            log.Warn(msg);
                        }
                        break;
                    case LogType.Error:
                        if (log.IsErrorEnabled)
                        {
                            log.Error(msg);
                        }
                        break;
                    case LogType.Fatal:
                        if (log.IsFatalEnabled)
                        {
                            log.Fatal(msg);
                        }
                        break;
                    default:
                        if (log.IsInfoEnabled)
                        {
                            log.Info(msg);
                        }
                        break;
                }

            }
            catch
            {

            }
        }

        #endregion ExceptionLogging

        #region GetDisplayMessage
        /// <summary>
        /// Gets the User friendly message from ExceptionResource.xml File
        /// First get the all the child nodes and get look for the specified node
        /// If that node is there then get the attribute called "Message"
        /// </summary>
        /// <param name="kamException">Object if KAM Exception Logger Class</param>
        /// <returns> returns User friendly message</returns>
        public static string GetDisplayMessage(KAMException kamException)
        {
            string message = LoggerConstants.STRING_EMPTY;
            //Avoid logging Thread Aborted exception in teh database
            //if (kamException.InnerException.GetType().FullName == LoggerConstants.THREAD_ABORTED)
            //    return message;

            ////return ExceptionResource.ResourceManager.GetString(kamException.InnerException.GetType().Name.ToString()).ToString();

            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(LoggerConstants.EXCEPTION_RESOURCE_FILE);

            //XmlNodeList nodeList;

            //nodeList = xmlDoc.ChildNodes;

            //XmlNode node;
            //if (nodeList != null && nodeList.Count > 0)
            //{
            //    // if the the specified errormessage is not there in the ExceptionResource.xml file then go with the DeafultException
            //    node = xmlDoc.ChildNodes.Item(1).SelectNodes(kamException.InnerException.GetType().Name.ToString()).Item(0);
            //    if (node != null)
            //    {
            //        message = node.Attributes.GetNamedItem(LoggerConstants.DISPLAY_MESSAGE).Value;
            //    }
            //    else
            //    {
            //        node = xmlDoc.ChildNodes.Item(1).SelectNodes(LoggerConstants.DEFAULT_EXCEPTION).Item(0);
            //        message = node.Attributes.GetNamedItem(LoggerConstants.DISPLAY_MESSAGE).Value;
            //    }
            //     //nodeList.Item(0).Attributes.GetNamedItem(LoggerConstants.DISPLAY_MESSAGE).Value.ToString();
            //}

            return message;
        }
        #endregion
        */
    }
}