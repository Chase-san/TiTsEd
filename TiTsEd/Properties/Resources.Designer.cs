﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TiTsEd.Properties
{
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources
    {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources()
        {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TiTsEd.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://forum.fenoxo.com/threads/titsed-a-save-editor.2809/.
        /// </summary>
        public static string ForumThreadUrl
        {
            get
            {
                return ResourceManager.GetString("ForumThreadUrl", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/algoRhythm99/TiTsEd/issues.
        /// </summary>
        public static string IssuesUrl
        {
            get
            {
                return ResourceManager.GetString("IssuesUrl", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://raw.githubusercontent.com/algoRhythm99/TiTsEd/master/latest.txt.
        /// </summary>
        public static string LatestFileUrl
        {
            get
            {
                return ResourceManager.GetString("LatestFileUrl", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to https://github.com/algoRhythm99/TiTsEd/releases.
        /// </summary>
        public static string ReleasesUrl
        {
            get
            {
                return ResourceManager.GetString("ReleasesUrl", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
        /// </summary>
        public static System.Drawing.Icon TiTsEd
        {
            get
            {
                object obj = ResourceManager.GetObject("TiTsEd", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
    }
}
