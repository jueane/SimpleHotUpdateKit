using System;

public static class DatetimeUtil
{
    public static string GetSimpleTimeOfCurrent()
    {
        DateTime now = DateTime.Now;
        string year = (now.Year % 100).ToString("00"); // 只取年份的后两位
        string month = now.Month.ToString("00");
        string day = now.Day.ToString("00");
        string hour = now.Hour.ToString("00");
        string minute = now.Minute.ToString("00");
        string second = now.Second.ToString("00");
        string formattedDateTime = $"{year}{month}{day}_{hour}{minute}{second}";
        return formattedDateTime;
    }
}