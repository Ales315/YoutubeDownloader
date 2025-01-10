# WPF YouTube Downloader

A simple WPF application designed for downloading YouTube videos using the [YouTubeExplode](https://github.com/Tyrrrz/YouTubeExplode) library. 
This application provides users with the ability to download videos with preferred quality and format and optionally extract video or audio. 

![Downloader demo](https://github.com/user-attachments/assets/0bb0011a-2270-4c35-a9ef-3c24dd0e3efc)


## Features

- Search videos by URL
- Extract audio or video.
- Save format preferences
- Light/Dark theme

## Getting started
- Download latest release
- Place the executable inside a folder
  
## How to Use
1. Paste the YouTube video URL into the search field.
3. Click the **Search** button to retrieve video information.
4. Select your desired resolution/format from the dropdown menu.
5. Click the **Download** button to begin downloading.

In settings page you can choose the output folder and video/audio format preferences.

With **Auto Download** option enabled videos will be directly downloaded using selected video/audio format preferences.

## Dependencies

- **[YouTubeExplode](https://www.nuget.org/packages/YouTubeExplode)**
- **[MaterialDesign in XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit)**

## Known Issues

- This application can only download public and non-DRM-protected videos.
- Playlists are not supported
- Subtitles and language audio tracks are not available

