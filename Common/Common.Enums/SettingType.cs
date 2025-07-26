using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum SettingType
{
    [Description("Файл robots.txt")]
    [Display(Name = "Файл robots.txt")]
    RobotsFile = 1,

    [Description("Файл sitemap.xml")]
    [Display(Name = "Файл sitemap.xml")]
    SitemapFile = 2,

    [Description("Хедер")]
    [Display(Name = "Хедер")]
    Header = 3,

    [Description("Футер")]
    [Display(Name = "Футер")]
    Footer = 4
}