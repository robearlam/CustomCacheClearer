using System;
using System.Linq;
using Sitecore.Events;
using Sitecore.Publishing;

namespace CustomHtmlCacheClearer
{
    public class HtmlCacheClearer : Sitecore.Publishing.HtmlCacheClearer
    {
        private const string SettingName = "CustomHtmlCacheClearer.IgnoreItems";

        public new void ClearCache(object sender, EventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            var sitecoreEventArgs = args as SitecoreEventArgs;
            if (sitecoreEventArgs == null || !sitecoreEventArgs.Parameters.Any())
            {
                return;
            }

            var publisher = sitecoreEventArgs.Parameters[0] as Publisher;
            if (publisher == null)
            {
                return;
            }

            if (IsCacheClearRequired(publisher.Options))
            {
                base.ClearCache(sender, args);
            }
        }

        protected bool IsCacheClearRequired(PublishOptions publishOptions)
        {
            var exemptPaths = Sitecore.Configuration.Settings.GetSetting(SettingName);
            if (string.IsNullOrWhiteSpace(exemptPaths))
            {
                return true;
            }

            return exemptPaths.Split('|')
                              .All(excemptPath => !publishOptions.RootItem.Paths.FullPath.StartsWith(excemptPath,StringComparison.OrdinalIgnoreCase));
        }
    }
}
