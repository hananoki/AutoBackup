#if UNITY_2019_1_OR_NEWER

using HananokiEditor.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using P = HananokiEditor.AutoBackup.SettingsProject;


namespace HananokiEditor.AutoBackup {

	public class Utils {

		/////////////////////////////////////////
		internal static void Install完了時の処理() {
			if( !P.i.m_backupPackageName.IsExistsDirectory() ) return;

			P.i.有効 = true;
			Core.Start();
			EditorApplication.update -= Install完了時の処理;
		}



		/////////////////////////////////////////
		internal static void バックアップパッケージをインストールする() {
			fs.mkdir( P.i.m_backupPackageName );
			var json = $@"{{
  ""name"": ""{Core.backupDomain}"",
  ""displayName"": ""{P.i.m_backupPackageName}"",
  ""version"": ""1.0.0"",
  ""unity"": ""2018.3"",
  ""description"": """"
}}
";

			fs.WriteAllText( $"{P.i.m_backupPackageName}/package.json", json );

			var meta = $@"fileFormatVersion: 2
guid: {GUIDUtils.NewString()}
PackageManifestImporter:
  externalObjects: {{ }}
  userData:
  assetBundleName:
  assetBundleVariant:
";
			try {
				fs.WriteAllText( $"{P.i.m_backupPackageName}/package.json.meta", meta );
			}
			catch( UnauthorizedAccessException e ) {
				if( UnitySymbol.UNITY_2019_3_OR_NEWER ) {
					throw e;
				}
				else {
					// UNITY_2019_2 だとアクセス権握ったままなので例外が来る
					// metaは対して重要ではないので無視
				}
			}
			PackageDatabaseUtils.InstallFromPath( $"{fs.currentDirectory}/{P.i.m_backupPackageName}/package.json" );
		}


		//internal static void OpenScene() {
		//	Debug.Log( "f8e194d23f81e17498d785c94ab93f57".ToAssetPath() );
		//	EditorSceneManager.OpenScene( $"{W.バックアップ先のアセットパス}/20210524_1729_34_AAA.unity" );
		//}


		/////////////////////////////////////////
		internal static void シーンをバックアップ( bool force = false ) {
			for( int i = 0; i < SceneManager.sceneCount; i++ ) {
				var p = SceneManager.GetSceneAt( i );
				if( !p.isDirty ) continue;
				var guid = p.path.ToGUID();
				EditorSceneManager.SaveScene( p, $"Packages/{Core.backupDomain}/{DateTime.Now.ToString( "yyyyMMdd_HHmm_ss" )}_{p.name} [{guid}].unity", true );
			}

			if( P.i.バックアップ時にSaveAssetsを呼ぶ ) {
				AssetDatabase.SaveAssets();
			}

			指定ファイル数を超えたら古いファイルを消す();
		}



		/////////////////////////////////////////
		internal static void 指定ファイル数を超えたら古いファイルを消す() {
			if( !P.i.指定ファイル数を超えたら古いファイルを消す ) return;

			Task.Run( () => {
				var _fileList = new Queue<string>();
				var aa = DirectoryUtils.GetFiles( P.i.m_backupPackageName, "*.unity", System.IO.SearchOption.TopDirectoryOnly );
				foreach( var p in aa ) {
					if( P.i.m_maxFileCount <= _fileList.Count ) {
						var nm = _fileList.Dequeue();
						fs.rm( $"{nm}.meta" );
						fs.rm( nm );
					}
					_fileList.Enqueue( p );
				}
				EditorApplication.delayCall += () => {
					AssetDatabase.Refresh();
				};
			} );
		}



		/////////////////////////////////////////
		internal static void バックアップをクリーン() {
			Task.Run( () => {
				var files = DirectoryUtils.GetFiles( P.i.m_backupPackageName, "*.unity", System.IO.SearchOption.TopDirectoryOnly );
				foreach( var p in files ) {
					fs.rm( $"{p}.meta" );
					fs.rm( p );
				}
				EditorApplication.delayCall += () => {
					AssetDatabase.Refresh();
				};
			} );
		}

	}
}

#endif
