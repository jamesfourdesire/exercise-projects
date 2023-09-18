using System;
using System.Globalization;
using System.Linq;

public interface IFDDateTime
{

	DateTime Today { get; }
	DateTime UtcNow { get; }
	DateTime Now { get; }
}

public class FDDateTime : IFDDateTime
{

	public DateTime Today => DateTime.Today;
	public DateTime UtcNow => DateTime.UtcNow;
	public DateTime Now => DateTime.Now;
}

public class FDDateTimeHack : IFDDateTime
{

	public TimeSpan Offset = new TimeSpan();

	public FDDateTimeHack() { }

	public DateTime Today => this.Now.Date;

	public DateTime UtcNow => DateTime.UtcNow + Offset;

	public DateTime Now => DateTime.Now + Offset;
}

// TODO: Need Rename
public static partial class FDDateTimeHelper
{

	public static readonly double DAY_INTERVAL = 86400.0;
	public static readonly double HOUR_INTERVAL = 3600.0;

	public static IFDDateTime DateTimeProvider = new FDDateTime();

	static readonly DateTime UTCDateTime1970 = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

	public static DateTime UTCDateTimeSince1970(double seconds) => UTCDateTime1970.AddSeconds(seconds);

	public static double TimeIntervalSince1970(this DateTime date)
	{

		TimeSpan diff = date.ToUniversalTime() - UTCDateTime1970;
		return diff.TotalSeconds;
	}

	public static double TimeIntervalSince1970FromTimeZone(this DateTime date, TimeZoneInfo timeZone) => (date - UTCDateTime1970).TotalSeconds - timeZone.BaseUtcOffset.TotalSeconds;

	public static double TimeIntervalSinceNow(this DateTime date) => (date.ToUniversalTime() - DateTimeProvider.UtcNow).TotalSeconds;

	public static double TimeIntervalSinceDate(this DateTime date, DateTime sinceDate) => (date.ToUniversalTime() - sinceDate.ToUniversalTime()).TotalSeconds;

	public static double NowSince1970() => (DateTimeProvider.UtcNow - UTCDateTime1970).TotalSeconds;

	public static DateTime Now() => DateTimeProvider.Now;

	public static DateTime Today() => DateTimeProvider.Today;

	public static bool IsToday(double secondsSince1970) => UTCDateTimeSince1970(secondsSince1970).IsToday();

	public static bool IsToday(this DateTime date) => date.ToLocalTime().Date == DateTimeProvider.Today;

	public static bool IsYesterday(double secondsSince1970) => UTCDateTimeSince1970(secondsSince1970).IsYesterday();

	public static bool IsYesterday(this DateTime date) => date.ToLocalTime().Date == DateTimeProvider.Today.AddDays(-1.0);

	#region TimeZone Handling
	public static string GetCurrentTimeZoneID() => TimeZoneInfo.Local.Id;

	public static double CurrentTimeZoneOffsetFromUTC() => TimeZoneInfo.Local.GetUtcOffset(Now()).TotalSeconds;

	public static TimeZoneInfo ToTimeZoneInfo(this string timeZoneID) => TimeZoneInfo.FindSystemTimeZoneById(timeZoneID);

	public static DateTime TimeZoneTimeToUTCTime(DateTime timeZoneTime, TimeZoneInfo timeZoneInfo)
	{

		var unspecified = DateTime.SpecifyKind(timeZoneTime, DateTimeKind.Unspecified);
		if (timeZoneInfo.IsInvalidTime(unspecified))
		{

			TimeSpan offsetYesterday = timeZoneInfo.GetUtcOffset(timeZoneTime.AddDays(-1)); // before time change
			TimeSpan offsetTomorrow = timeZoneInfo.GetUtcOffset(timeZoneTime.AddDays(1));   // after time change
			double minOffset = offsetTomorrow.TotalMinutes - offsetYesterday.TotalMinutes;
			unspecified = timeZoneTime.AddMinutes(minOffset);
		}
		return TimeZoneInfo.ConvertTimeToUtc(unspecified, timeZoneInfo);
	}

	public static DateTime ToTimeZoneTime(this DateTime date)
	{

		var utcDate = date.ToUniversalTime();
		var timeZoneTime = DateTime.SpecifyKind(
			TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.Local),
			DateTimeKind.Local
		);

		return timeZoneTime;
	}

	public static DateTime ToTimeZoneTime(this DateTime date, TimeZoneInfo timeZoneInfo)
	{

		var utcDate = date.ToUniversalTime();
		var timeZoneTime = DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(utcDate, timeZoneInfo), DateTimeKind.Local);

		return timeZoneTime;
	}
	#endregion

	#region Day-related
	public static double DayStartToday() => DateTimeProvider.Today.TimeIntervalSince1970();

	public static DateTime DayStart(double secondsSince1970) => UTCDateTimeSince1970(secondsSince1970).DayStart();

	public static DateTime DayStart(this DateTime date) => date.ToLocalTime().Date;

	public static DateTime Tomorrow(double secondsSince1970) => UTCDateTimeSince1970(secondsSince1970).Tomorrow();

	public static DateTime Tomorrow(this DateTime date) => date.ToLocalTime().AddDays(1.0).Date;

	public static DateTime EndOfDay(this DateTime date) => date.ToLocalTime().DayStart().AddDays(1).AddMilliseconds(-1);
	#endregion

	#region Week-related
	public static DateTime WeekStart(double secondsSince1970) => UTCDateTimeSince1970(secondsSince1970).WeekStart(DayOfWeek.Sunday);

	public static DateTime WeekStart(this DateTime date, int firstDayOfWeek, DateTimeKind dateTimeKind = DateTimeKind.Local)
	{

		var shift = (int)date.DayOfWeek - firstDayOfWeek;

		if (shift < 0)
		{
			shift += 7;
		}

		if (dateTimeKind == DateTimeKind.Local)
		{
			return date.DayStart().AddDays(-shift);
		}

		return date.Date.AddDays(-shift);
	}

	public static DateTime WeekStart(this DateTime date, DayOfWeek firstDayOfWeek, DateTimeKind dateTimeKind = DateTimeKind.Local)
	{

		var shift = (int)date.DayOfWeek - (int)firstDayOfWeek;

		if (shift < 0)
		{
			shift += 7;
		}

		if (dateTimeKind == DateTimeKind.Local)
		{
			return date.DayStart().AddDays(-shift);
		}

		return date.Date.AddDays(-shift);
	}

	public static DateTime WeekEnd(this DateTime date, DayOfWeek weekStart)
	{

		var weekEnd = (((int)weekStart) - 1 + 7) % 7;
		var localTime = date.ToLocalTime();
		var dayOfWeek = (int)localTime.DayOfWeek;

		if (dayOfWeek == weekEnd)
		{
			return date.Date;
		}
		else
		{
			return date.AddDays((weekEnd - dayOfWeek) % 7).Date;
		}
	}

	public static int GetWeekOfYear(this DateTime date, DayOfWeek firstDayOfWeek, CalendarWeekRule weekRule = CalendarWeekRule.FirstDay)
	{

		CultureInfo cul = CultureInfo.CurrentCulture;

		int weekNumber = cul.Calendar.GetWeekOfYear(
			date,
			weekRule,
			firstDayOfWeek
		);

		return weekNumber;
	}
	#endregion

	#region Month-related
	public static DateTime MonthStart(this DateTime date, DateTimeKind dateTimeKind = DateTimeKind.Local) => new(date.Year, date.Month, 1, 0, 0, 0, dateTimeKind);

	public static DateTime LastDayOfMonth(this DateTime date) => new(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
	#endregion

	#region Year-related
	public static DateTime YearStart(this DateTime date, DateTimeKind dateTimeKind = DateTimeKind.Local) => new(date.Year, 1, 1, 0, 0, 0, dateTimeKind);

	public static DateTime YearEnd(this DateTime date, DateTimeKind dateTimeKind = DateTimeKind.Local) => new(date.Year, 12, 31, 0, 0, 0, dateTimeKind);

	public static DateTime MaxValueMinusOneYear() => DateTime.MaxValue.AddYears(-1);
	#endregion

	#region Others
	public static bool IsNowInRange(double start, double end)
	{

		var now = NowSince1970();

		return start <= now && (end <= 0 || now < end);
	}

	public static string RemainedTime(double durationSeconds)
	{

		int seconds = (int)(durationSeconds % 60);
		int mins = (int)(durationSeconds / 60) % 60;
		int hours = (int)(durationSeconds / 3600) % 24;
		int days = (int)(durationSeconds / 86400);

		return days > 0 ? string.Format("{0:00}D {1:00}:{2:00}:{3:00}", days, hours, mins, seconds) : string.Format("{0:00}:{1:00}:{2:00}", hours, mins, seconds);
	}

	public static string ToNumericDateString(this DateTime date, bool showYear = true, bool showShortYear = false)
	{

		var dateSeparator = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator;
		var dates = date.ToShortDateString().Split(new string[] { dateSeparator }, StringSplitOptions.None).ToList();
		var yearIdx = dates.IndexOf(date.ToString("yyyy"));

		if (showYear && showShortYear)
		{

			dates[yearIdx] = date.ToString("yy");
		}
		else if (!showYear)
		{

			dates.RemoveAt(yearIdx);
		}

		return string.Join(dateSeparator, dates);
	}
	#endregion

	#region Utilities
	static TimeSpan ToTimeSpan(this double seconds) => TimeSpan.FromSeconds(seconds);
	#endregion
}
