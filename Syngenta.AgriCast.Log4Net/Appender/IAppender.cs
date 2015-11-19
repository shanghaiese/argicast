#region Copyright & License
//
// Copyright 2001-2005 The Apache Software Foundation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

#region Change History
// Modified On: 25-JUL-07
//
// Modified By: MOHAN P
//
// Purpose    : Initialized a property called CustomError To catch an exception if there is any problem in connecting to database
//              in Reading a File and in Sending an Email
//
#endregion

using log4net.Filter;
using log4net.Layout;
using log4net.Core;

namespace log4net.Appender
{
	/// <summary>
	/// Implement this interface for your own strategies for printing log statements.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Implementors should consider extending the <see cref="AppenderSkeleton"/>
	/// class which provides a default implementation of this interface.
	/// </para>
	/// <para>
	/// Appenders can also implement the <see cref="IOptionHandler"/> interface. Therefore
	/// they would require that the <see cref="IOptionHandler.ActivateOptions()"/> method
	/// be called after the appenders properties have been configured.
	/// </para>
	/// </remarks>
	/// <author>Nicko Cadell</author>
	/// <author>Gert Driesen</author>
	public interface IAppender
	{
		/// <summary>
		/// Closes the appender and releases resources.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Releases any resources allocated within the appender such as file handles, 
		/// network connections, etc.
		/// </para>
		/// <para>
		/// It is a programming error to append to a closed appender.
		/// </para>
		/// </remarks>
		void Close();

		/// <summary>
		/// Log the logging event in Appender specific way.
		/// </summary>
		/// <param name="loggingEvent">The event to log</param>
		/// <remarks>
		/// <para>
		/// This method is called to log a message into this appender.
		/// </para>
		/// </remarks>
		void DoAppend(LoggingEvent loggingEvent);

		/// <summary>
		/// Gets or sets the name of this appender.
		/// </summary>
		/// <value>The name of the appender.</value>
		/// <remarks>
		/// <para>The name uniquely identifies the appender.</para>
		/// </remarks>
		string Name { get; set; }

        //BEGIN ADDED FOR RMS TO CATCH AN EXCEPTION BY MOHAN ON 25-JUL-07
        string CustomError { get; set; }
        //END ADDED FOR RMS TO CATCH AN EXCEPTION BY MOHAN ON 25-JUL-07
	}
}