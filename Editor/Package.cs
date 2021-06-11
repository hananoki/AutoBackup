
using System;
using UnityEditor;

namespace HananokiEditor.AutoBackup {
  public static class Package {
    public const string reverseDomainName = "com.hananoki.auto-backup";
    public const string name = "AutoBackup";
    public const string nameNicify = "Auto Backup";
    public const string editorPrefName = "Hananoki.AutoBackup";
    public const string version = "0.1.0";
    public static string projectSettingsPath => $"{SharedModule.SettingsEditor.projectSettingDirectory}/AutoBackup.json";
  }
}
