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

---

## 🛠 セットアップとビルド方法

### 1. リポジトリのクローン
ターミナルを開き、任意の作業ディレクトリで以下のコマンドを実行します。

```bash
git clone https://github.com/ararobo/RobotControllerForUnityROS2.git
```

### 2. Unity Hubへのプロジェクト追加
1. **Unity Hub** を起動します。
2. 「プロジェクト」タブの右上にある **「追加」** ボタンをクリックします。
3. クローンした `RobotControllerForUnityROS2` フォルダを選択して開きます。
4. プロジェクト一覧に追加されたら、Unityエディタを起動します。

### 3. Android向けビルド (APK作成)
1. Unityエディタ上部メニューから **File > Build Settings...** を開きます。
2. **Platform** 一覧から **Android** を選択し、**Switch Platform** をクリックします。
3. **Build** ボタンをクリックし、保存先とファイル名（例: `RobotController.apk`）を指定してAPKを生成します。

### 4. 端末へのインストール
1. 生成された `.apk` ファイルをAndroid端末に転送します。
2. 端末のファイルマネージャーからファイルを開き、インストールを実行します。

---
