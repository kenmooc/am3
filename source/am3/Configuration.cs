#region License, Terms and Author(s)
//
// AM3 - ASP.NET Maintenance Mode Module
// Copyright (c) 2009 Raihan Iqbal. All rights reserved.
//
//  Author(s):
//
//      Raihan Iqbal, http://www.raihaniqbal.org
//
// This library is free software; you can redistribute it and/or modify it 
// under the terms of the New BSD License, a copy of which should have 
// been delivered along with this distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
#endregion

namespace AM3
{
    #region Imports

    using System.Collections.Specialized;
    using System.Configuration;

    #endregion

    internal sealed class Configuration
    {
        internal const string GroupName = "am3";
        internal const string GroupSlash = GroupName + "/";

        public static NameValueCollection AppSettings
        {
            get
            {
#if NET_1_0 || NET_1_1
                return ConfigurationSettings.AppSettings;
#else
                return ConfigurationManager.AppSettings;
#endif
            }
        }

        public static object GetSubsection(string name)
        {
            return GetSection(GroupSlash + name);
        }

        public static object GetSection(string name)
        {
#if NET_1_0 || NET_1_1
            return ConfigurationSettings.GetConfig(name);
#else
            return ConfigurationManager.GetSection(name);
#endif
        }

        private Configuration() { }
    }
}
