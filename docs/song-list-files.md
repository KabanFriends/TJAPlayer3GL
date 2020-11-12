<!-- omit in toc -->
# Song List Files

Required: TJAPlayer3 v4.7.0 or greater

<!-- omit in toc -->
## Table of Contents

- [Overview](#overview)
  - [Example Uses](#example-uses)
- [Song List File Extension & Format](#song-list-file-extension--format)
- [Song List File Content](#song-list-file-content)
  - [Example Song List File Content](#example-song-list-file-content)
- [Song List File Organization](#song-list-file-organization)
- [Creating Song List Files](#creating-song-list-files)

## Overview

You might want one song to appear in multiple locations within the song selection tree. You might want a set of songs to appear in a completely customized order. Instead of copying files and renaming them to control the order in which they appear, you can create and use Song List Files.

### Example Uses

- Discographies by composer, producer, musician, vocalist, doujin circle, etc., ordered by release date.
- Sets of songs from specific arcade or console rhythm game versions, displayed in their expected order.

## Song List File Extension & Format

A Song List File is a simple text file:

- The file extension is .t3u8
- The file encoding is UTF-8. If any included .tja file paths contain non-ASCII characters, be sure to instruct your text editor to save the file in the UTF-8 encoding.

## Song List File Content

Each line of a Song List File references the path to a .tja file. The following types of paths are supported:

- Relative paths, which are recommended
- Absolute paths

### Example Song List File Content

    FolderA\Filename1.tja
    ..\FolderB\Filename2.tja
    D:\FolderC\Filename3.tja

When loaded by TJAPlayer3, these songs will be listed in the exact order in which they appear in the Song List File.

**Future:** TJAPlayer3 sorting and/or filtering enhancements may in the future allow for Song List sorting to be overridden and/or for Song List items to be filtered from view.

## Song List File Organization

Use box.def files to create folder hierarchies within the song selection screen support songs from Song List Files.

For best results place only two files, a box.def file and a Song List File, side by side in a new folder:

    D:\TJA\My favorite vocalist\
      box.def
      My favorite vocalist.t3u8

*Note:* The name of the containing folder and of the Song List File itself _do not matter_. The sibling or ancestor box.def file is responsible for all organization in the song selection screen, including its usual support for titling the folder, setting its genre, etc.

*Note:* If multiple Song List Files exist within a single folder, the Song List Files are parsed in ascending order of their file names.

## Creating Song List Files

Song List Files can be created many ways:

- Create them by hand
- Generate them with small scripts or full-fledged programs

The only limit is your imagination.
