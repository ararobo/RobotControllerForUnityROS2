# RobotControllerForUnityROS2
ROS 2とUnityを連携させたロボット制御用コントローラーです。

## 🚀 使用技術

このプロジェクトは、以下の技術とツールを使用して開発されています。

### フレームワーク & ライブラリ

| 技術名 | アイコン | 説明 |
| :--- | :---: | :--- |
| **Unity** | <img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white"> | ロボットの可視化と制御インターフェースを提供するためのプラットフォーム。 |
| **ROS 2 Humble** | <img src="https://img.shields.io/badge/ROS2-F4F1DE?style=for-the-badge&logo=ros&logoColor=black"> | ロボット通信の中核となる分散制御システム。**Humble** に対応。 |
| **ROS 2 for Unity** | <img src="https://img.shields.io/badge/ROS2_for_Unity-000000?style=for-the-badge&logo=unity&logoColor=white"> | UnityとROS 2のメッセージ通信を可能にするライブラリ。 |

### プログラミング言語

| 技術名 | アイコン | 説明 |
| :--- | :---: | :--- |
| **C#** | <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white"> | Unityスクリプトの作成に使用。 |
| **Python** | <img src="https://img.shields.io/badge/Python-3776AB?style=for-the-badge&logo=python&logoColor=white"> | ROS 2ノードの開発に使用。 |

### 開発環境 & ツール

| 技術名 | アイコン | 説明 |
| :--- | :---: | :--- |
| **Visual Studio Code** | <img src="https://img.shields.io/badge/VS%20Code-007ACC?style=for-the-badge&logo=visual-studio-code&logoColor=white"> | コード編集に使用。 |
| **Git** | <img src="https://img.shields.io/badge/Git-F05032?style=for-the-badge&logo=git&logoColor=white"> | バージョン管理に使用。 |
| **GitHub** | <img src="https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white"> | リポジトリホスティングに使用。 |

🛠 セットアップとビルド方法
このプロジェクトをビルドしてAndroid端末で実行するまでの手順は以下の通りです。

1. リポジトリのクローン
ターミナル（またはGit Bash）を開き、プロジェクトをローカル環境にクローンします。

Bash
# クローンを実行するディレクトリ（例: ~/UnityProjects）に移動
cd ~/UnityProjects

# リポジトリをクローン
git clone https://github.com/ararobo/RobotControllerForUnityROS2.git

2. Unity Hubへのプロジェクト追加
Unity Hub を起動します。

「プロジェクト」タブの右上にある 「追加」 ボタン（または「ディスクから加える」）をクリックします。

先ほどクローンした RobotControllerForUnityROS2 フォルダを選択して開きます。

プロジェクト一覧に追加されたら、クリックしてUnityエディタを起動します。

3. Android向けビルド (APK作成)
Unityエディタの上部メニューから File（ファイル） > Build Settings...（ビルド設定） を開きます。

Platform 一覧から Android を選択し、Switch Platform をクリックします（既にAndroidになっている場合は不要です）。

Build ボタンをクリックし、保存先とファイル名（例: RobotController.apk）を指定してAPKファイルを生成します。

4. Android端末へのインストール
生成された .apk ファイルをAndroid端末に転送します（USB接続、Googleドライブ、Slack経由など）。

Android端末側でファイルを開き、インストールを実行します。
