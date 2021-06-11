using HananokiRuntime.Extensions;
using System;
using System.IO;
using UnityEngine;
using P = HananokiEditor.AutoBackup.SettingsProject;


namespace HananokiEditor.AutoBackup {

	[Serializable]
	public class SettingsProject {
		public static P i;

		public string m_backupPackageName = "Backup";
		public int m_maxFileCount = 20;
		public int m_BackupInterval_Minutes = 5;


		#region Flags

		public int m_flag;
		public const int _有効 = ( 1 << 0 );
		public const int _Backupscenesを退避する = ( 1 << 1 );
		public const int _バックアップ時にSaveAssetsを呼ぶ = ( 1 << 2 );
		public const int _指定ファイル数を超えたら古いファイルを消す = ( 1 << 3 );


		public bool 有効 {
			get => m_flag.Has( _有効 );
			set => m_flag.Toggle( _有効, value );
		}
		public bool Backupscenesを退避する {
			get => m_flag.Has( _Backupscenesを退避する );
			set => m_flag.Toggle( _Backupscenesを退避する, value );
		}
		public bool バックアップ時にSaveAssetsを呼ぶ {
			get => m_flag.Has( _バックアップ時にSaveAssetsを呼ぶ );
			set => m_flag.Toggle( _バックアップ時にSaveAssetsを呼ぶ, value );
		}
		public bool 指定ファイル数を超えたら古いファイルを消す {
			get => m_flag.Has( _指定ファイル数を超えたら古いファイルを消す );
			set => m_flag.Toggle( _指定ファイル数を超えたら古いファイルを消す, value );
		}

		#endregion


		#region Methods

		public static void Load() {
			if( i != null ) return;

			i = JsonUtility.FromJson<P>( fs.ReadAllText( Package.projectSettingsPath ) );
			if( i == null ) {
				i = new P();
				Save();
			}
		}


		public static void Save() {
			File.WriteAllText( Package.projectSettingsPath, JsonUtility.ToJson( i, true ) );
		}

		#endregion

	}
}
