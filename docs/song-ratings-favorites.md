<!-- omit in toc -->
# Song Ratings / Favorites

Required: TJAPlayer3 v4.7.0 or greater

<!-- omit in toc -->
## Table of Contents

- [Overview](#overview)
- [The Five-Point Rating Scale](#the-five-point-rating-scale)
- [Setting And Removing Ratings](#setting-and-removing-ratings)
- [Sorting And Filtering Using Ratings](#sorting-and-filtering-using-ratings)
- [Skin Support For Ratings](#skin-support-for-ratings)

## Overview

The song selection screen allows you to assign a rating to each song and to then sort / filter your view to more easily find and play songs of the ratings (or ranges of ratings) you want. You may also remove ratings from songs.

## The Five-Point Rating Scale

Each song can be assigned a rating from 1 through 5. Use as few or as many of the available rating values as you wish. For example:

- Assign a single point, or remove it, akin to the favorites feature of the official taiko rhythm games
- Use all five values, as you would rate songs in a music player

*Note:* However many rating values you choose to use, consider treating lower ratings as less preferred and higher ratings as more preferred. Doing so will likely align better with how skins display ratings, how other TJAPlayer3 features combine with the ratings / favorites functions, etc.

## Setting And Removing Ratings

To set or change the rating of the selected song, hold down the `Ctrl` key
and then press and release the appropriate key `1` through `5` (across the top of your keyboard.) Finally, release the `Ctrl` key.

To remove the rating from the selected song, hold down the `Ctrl` key and then press and release one of the following keys before finally releasing the `Ctrl` key:

- the `0` key (across the top of your keyboard.)
- the number key (across the top of your keyboard) of the current rating. For example, if the song is currently rated `4`, pressing and releasing `4` (while pressing `Ctrl`) will remove the rating.

## Sorting And Filtering Using Ratings

As of TJAPlayer3 v4.7.0, Song Ratings / Favorites have been added to the set of available choices when sorting songs.

*Future:* Future versions of TJAPlayer3 will include sorting and filtering improvements, but until then you may use sorting to:

- group / ungroup rated / unrated songs and
- sort rated songs in various orders

## Skin Support For Ratings

The default skin included with TJAPlayer3 includes full support for displaying Song Ratings. Other skins you create or download need to be updated to support ratings. (Ratings still function without skin support, though invisibly.)

Skinning for TJAPlayer3 is currently without much documentation. As of TJAPlayer3 v4.7.0, to get an idea of which skin config keys affect the display of ratings, browse to the following source file and search for `SongSelect_Rating`: https://github.com/twopointzero/TJAPlayer3/blob/master/TJAPlayer3/Common/CSkin.cs#L2531

The skin config keys found there are:

- offsets relative to the song's panel in the song selection screen
- interval(s) relative to the other drawn rating indicators.

The rating indicator image itself is called `Rating.png`. The default skin image is generated from the `Rating.svg` Inkscape SVG document found alongside it in the skin's `Graphics\3_SongSelect` folder.