﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FinderNET.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AddonsLocale {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AddonsLocale() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("FinderNET.Resources.AddonsLocale", typeof(AddonsLocale).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static string AddonsEmbedList_title {
            get {
                return ResourceManager.GetString("AddonsEmbedList_title", resourceCulture);
            }
        }
        
        internal static string AddonsNotInstalled {
            get {
                return ResourceManager.GetString("AddonsNotInstalled", resourceCulture);
            }
        }
        
        internal static string AddonsInstalled {
            get {
                return ResourceManager.GetString("AddonsInstalled", resourceCulture);
            }
        }
        
        internal static string AddonsEmbedInstall_title {
            get {
                return ResourceManager.GetString("AddonsEmbedInstall_title", resourceCulture);
            }
        }
        
        internal static string AddonsEmbed_fieldAddonName {
            get {
                return ResourceManager.GetString("AddonsEmbed_fieldAddonName", resourceCulture);
            }
        }
        
        internal static string AddonsEmbed_fieldAddonValue {
            get {
                return ResourceManager.GetString("AddonsEmbed_fieldAddonValue", resourceCulture);
            }
        }
        
        internal static string AddonsError_alreadyInstalled {
            get {
                return ResourceManager.GetString("AddonsError_alreadyInstalled", resourceCulture);
            }
        }
        
        internal static string AddonsError_notInstalled {
            get {
                return ResourceManager.GetString("AddonsError_notInstalled", resourceCulture);
            }
        }
        
        internal static string AddonsEmbedUninstall_title {
            get {
                return ResourceManager.GetString("AddonsEmbedUninstall_title", resourceCulture);
            }
        }
    }
}
