#if UNITY_2019_1_OR_NEWER

using HananokiEditor.Extensions;
using HananokiRuntime.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;

#if UNITY_2020_1_OR_NEWER
using UnityEditor.Localization;
[assembly: Localization]
#elif UNITY_2019_3_OR_NEWER
using UnityEditor.Localization.Editor;
[assembly: Localization]
#endif


namespace HananokiEditor.AutoBackup {
	public class Menu {
		const string m = "Tools/AutoBackup/";
		const string m1 = "Open Latest Backup";
		const string m2 = "Restore__Backupscenes";


		/////////////////////////////////////////
		[MenuItem( m + m1, false, 0 )]
		static void Open_Latest_Backup() {
			var files = DirectoryUtils.GetFiles( $"{Core.バックアップ先のアセットパス}", "*.unity", System.IO.SearchOption.TopDirectoryOnly );
			EditorSceneManager.OpenScene( files.Last() );
		}

		[MenuItem( m + m1, true )]
		static bool Open_Latest_Backup_Check() {
			var files = DirectoryUtils.GetFiles( $"{Core.バックアップ先のアセットパス}", "*.unity", System.IO.SearchOption.TopDirectoryOnly );
			return 0 < files.Length ;
		}


		/////////////////////////////////////////
		[MenuItem( m + m2, false, 1 )]
		static void Tools_AutoBackup_Restore___Backupscenes() {
			var files = DirectoryUtils.GetFiles( $"{Core.バックアップ先のアセットパス}/__Backupscenes", "*.unity" );
			for( int i = 0; i < files.Length; i++ ) {
				var f = files[ i ];
				if( i == 0 ) {
					EditorSceneManager.OpenScene( f );
				}
				else {
					EditorSceneManager.OpenScene( f, OpenSceneMode.Additive );
				}
			}
		}



		[MenuItem( m + m2, true )]
		static bool Tools_AutoBackup_Restore__Backupscenes_Check() {
			if( !$"{Core.バックアップ先のアセットパス}/__Backupscenes".IsExistsDirectory() ) return false;
			return true;
		}

	}
}




#endif
