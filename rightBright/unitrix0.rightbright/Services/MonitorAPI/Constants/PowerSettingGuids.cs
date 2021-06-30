using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace unitrix0.rightbright.Services.MonitorAPI.Constants
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/windows/win32/power/power-setting-guids
    /// </summary>
    public static class PowerSettingGuids
    {
        /// <summary>
        /// The system power source has changed. The Data member is a DWORD with values
        /// from the SYSTEM_POWER_CONDITION enumeration that indicates the current power source.
        /// </summary>
        public static Guid GUID_ACDC_POWER_SOURCE = new("5D3E9A59-E9D5-4B00-A6BD-FF34FF516548");
        /// <summary>
        /// The remaining battery capacity has changed. The granularity varies from system to
        /// system but the finest granularity is 1 percent. The Data member is a DWORD that
        /// indicates the current battery capacity remaining as a percentage from 0 through 100.
        /// </summary>
        public static Guid GUID_BATTERY_PERCENTAGE_REMAINING = new("A7AD8041-B45A-4CAE-87A3-EECBB468A9E1");
        /// <summary>
        /// The current monitor's display state has changed. The Data member is a DWORD with one
        /// of the following values.
        /// </summary>
        public static Guid GUID_CONSOLE_DISPLAY_STATE = new("6FE69556-704A-47A0-8F24-C28D936FDA47");
        /// <summary>
        /// The user status associated with any session has changed. This represents the combined
        /// status of user presence across all local and remote sessions on the system.
        /// <remarks>
        /// This notification is sent only services and other programs running in session 0.
        /// User-mode applications should register for GUID_SESSION_USER_PRESENCE instead.
        /// </remarks>
        /// </summary>
        public static Guid GUID_GLOBAL_USER_PRESENCE = new("786E8A1D-B427-4344-9207-09E70BDCBEA9");
        /// <summary>
        /// The system is busy.
        /// </summary>
        public static Guid GUID_IDLE_BACKGROUND_TASK = new("515C31D8-F734-163D-A0FD-11A08C91E8F1");
        /// <summary>
        /// The primary system monitor has been powered on or off. The Data member is a DWORD that
        /// indicates the current monitor state.
        /// </summary>
        public static Guid GUID_MONITOR_POWER_ON = new("02731015-4510-4526-99E6-E5A17EBD1AEA");
        /// <summary>
        /// Battery saver has been turned off or on in response to changing power conditions.
        /// The Data member is a DWORD that indicates battery saver state.
        /// </summary>
        public static Guid GUID_POWER_SAVING_STATUS = new("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");
        /// <summary>
        /// The active power scheme personality has changed. All power schemes map to one of these
        /// personalities. The Data member is a GUID that indicates the new active power scheme personality.
        /// </summary>
        public static Guid GUID_POWERSCHEME_PERSONALITY = new("245d8541-3943-4422-b025-13A784F679B7");
        /// <summary>
        /// The display associated with the application's session has been powered on or off.
        /// </summary>
        public static Guid GUID_SESSION_DISPLAY_STATUS = new("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");
        /// <summary>
        /// The user status associated with the application's session has changed.
        /// </summary>
        public static Guid GUID_SESSION_USER_PRESENCE = new("3C0F4548-C03F-4c4d-B9F2-237EDE686376");

        public static Dictionary<Guid, string> Names = new Dictionary<Guid, string>()
        {
            {GUID_ACDC_POWER_SOURCE, nameof(GUID_ACDC_POWER_SOURCE)},
            {GUID_BATTERY_PERCENTAGE_REMAINING, nameof(GUID_BATTERY_PERCENTAGE_REMAINING)},
            {GUID_CONSOLE_DISPLAY_STATE, nameof(GUID_CONSOLE_DISPLAY_STATE)},
            {GUID_GLOBAL_USER_PRESENCE, nameof(GUID_GLOBAL_USER_PRESENCE)},
            {GUID_IDLE_BACKGROUND_TASK, nameof(GUID_IDLE_BACKGROUND_TASK)},
            {GUID_MONITOR_POWER_ON, nameof(GUID_MONITOR_POWER_ON)},
            {GUID_POWER_SAVING_STATUS, nameof(GUID_POWER_SAVING_STATUS)},
            {GUID_POWERSCHEME_PERSONALITY, nameof(GUID_POWERSCHEME_PERSONALITY)},
            {GUID_SESSION_DISPLAY_STATUS, nameof(GUID_SESSION_DISPLAY_STATUS)},
            {GUID_SESSION_USER_PRESENCE, nameof(GUID_SESSION_USER_PRESENCE)}
        };
    }
}