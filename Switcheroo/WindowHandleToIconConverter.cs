using Microsoft.Win32;
using Switcheroo.Core;
using System;
using System.Globalization;
using System.Runtime.Caching;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Switcheroo
{
    public class WindowHandleToIconConverter : IValueConverter
    {
        private readonly IconToBitmapImageConverter _iconToBitmapConverter;

        public WindowHandleToIconConverter()
        {
            _iconToBitmapConverter = new IconToBitmapImageConverter();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var handle = (IntPtr) value;
            var key = "IconImage-" + handle;
            var shortCacheKey = key + "-shortCache";
            var longCacheKey = key + "-longCache";
            var iconImage = MemoryCache.Default.Get(shortCacheKey) as BitmapImage;
            if (iconImage == null)
            {
                var window = new AppWindow(handle);
                var icon = ShouldUseSmallTaskbarIcons() ? window.SmallWindowIcon : window.LargeWindowIcon;
                iconImage = _iconToBitmapConverter.Convert(icon) ?? new BitmapImage();
                MemoryCache.Default.Set(shortCacheKey, iconImage, DateTimeOffset.Now.AddSeconds(5));
                MemoryCache.Default.Set(longCacheKey, iconImage, DateTimeOffset.Now.AddMinutes(120));
            }
            return iconImage;
        }

        private static bool ShouldUseSmallTaskbarIcons()
        {
            var cacheKey = "SmallTaskbarIcons";

            var cachedSetting = MemoryCache.Default.Get(cacheKey) as bool?;
            if (cachedSetting != null)
            {
                return cachedSetting.Value;
            }

            using (
                var registryKey =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced"))
            {
                if (registryKey == null)
                {
                    return false;
                }

                var value = registryKey.GetValue("TaskbarSmallIcons");
                if (value == null)
                {
                    return false;
                }

                int intValue;
                int.TryParse(value.ToString(), out intValue);
                var smallTaskbarIcons = intValue == 1;
                MemoryCache.Default.Set(cacheKey, smallTaskbarIcons, DateTimeOffset.Now.AddMinutes(120));
                return smallTaskbarIcons;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}