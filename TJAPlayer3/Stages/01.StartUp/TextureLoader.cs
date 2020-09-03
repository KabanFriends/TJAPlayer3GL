using System;
using System.Collections.Generic;
using System.Linq;
using FDK;

namespace TJAPlayer3
{
    class TextureLoader
    {
        const string BASE = @"Graphics\";

        // Stage
        const string TITLE = @"1_Title\";
        const string CONFIG = @"2_Config\";
        const string SONGSELECT = @"3_SongSelect\";
        const string SONGLOADING = @"4_SongLoading\";
        const string GAME = @"5_Game\";
        const string RESULT = @"6_Result\";
        const string EXIT = @"7_Exit\";

        // InGame
        const string CHARA = @"1_Chara\";
        const string DANCER = @"2_Dancer\";
        const string MOB = @"3_Mob\";
        const string COURSESYMBOL = @"4_CourseSymbol\";
        const string BACKGROUND = @"5_Background\";
        const string TAIKO = @"6_Taiko\";
        const string GAUGE = @"7_Gauge\";
        const string FOOTER = @"8_Footer\";
        const string END = @"9_End\";
        const string EFFECTS = @"10_Effects\";
        const string BALLOON = @"11_Balloon\";
        const string LANE = @"12_Lane\";
        const string GENRE = @"13_Genre\";
        const string GAMEMODE = @"14_GameMode\";
        const string FAILED = @"15_Failed\";
        const string RUNNER = @"16_Runner\";
        const string PUCHICHARA = @"18_PuchiChara\";
        const string DANC = @"17_DanC\";

        // InGame_Effects
        const string FIRE = @"Fire\";
        const string HIT = @"Hit\";
        const string ROLL = @"Roll\";
        const string SPLASH = @"Splash\";

        private readonly List<CTexture> _trackedTextures = new List<CTexture>();
        private readonly Dictionary<string, CTexture> _genreTexturesByFileNameWithoutExtension = new Dictionary<string, CTexture>();

        private (int skinGameCharaPtnNormal, CTexture[] charaNormal) TxCFolder(string folder)
        {
            var count = TJAPlayer3.t連番画像の枚数を数える(CSkin.Path(BASE + folder));
            var texture = count == 0 ? null : TxC(count, folder + "{0}.png");
            return (count, texture);
        }

        private CTexture[] TxC(int count, string format, int start = 0)
        {
            return TxC(format, Enumerable.Range(start, count).Select(o => o.ToString()).ToArray());
        }

        private CTexture[] TxC(string format, params string[] parts)
        {
            return parts.Select(o => TxC(string.Format(format, o))).ToArray();
        }

        private CTexture TxC(string path)
        {
            return Track(TxCUntracked(path));
        }

        private CTextureAf TxCAf(string path)
        {
            return Track(TxCAfUntracked(path));
        }

        private T Track<T>(T texture) where T : CTexture
        {
            if (texture != null)
            {
                _trackedTextures.Add(texture);
            }

            return texture;
        }

        internal CTexture TxCGenre(string fileNameWithoutExtension)
        {
            if (_genreTexturesByFileNameWithoutExtension.TryGetValue(fileNameWithoutExtension, out var texture))
            {
                return texture;
            }

            texture = TxC($"{GAME}{GENRE}{fileNameWithoutExtension}.png");

            _genreTexturesByFileNameWithoutExtension.Add(fileNameWithoutExtension, texture);

            return texture;
        }

        internal CTexture TxCUntracked(string path)
        {
            return TJAPlayer3.tテクスチャの生成(CSkin.Path(BASE + path));
        }

        private CTextureAf TxCAfUntracked(string path)
        {
            return TJAPlayer3.tテクスチャの生成Af(CSkin.Path(BASE + path));
        }

        public void Load()
        {
            #region 共通
            Tile_Black = TxC("Tile_Black.png");
            Tile_White = TxC("Tile_White.png");
            Menu_Title = TxC("Menu_Title.png");
            Menu_Highlight = TxC("Menu_Highlight.png");
            Enum_Song = TxC("Enum_Song.png");
            Scanning_Loudness = TxC("Scanning_Loudness.png");
            Overlay = TxC("Overlay.png");

            NamePlate = TxC(2, "{0}P_NamePlate.png", 1);
            #endregion
            #region 1_タイトル画面
            Title_Background = TxC($"{TITLE}Background.png");
            Title_Menu = TxC($"{TITLE}Menu.png");
            #endregion

            #region 2_コンフィグ画面
            Config_Background = TxC($"{CONFIG}Background.png");
            Config_Cursor = TxC($"{CONFIG}Cursor.png");
            Config_ItemBox = TxC($"{CONFIG}ItemBox.png");
            Config_Arrow = TxC($"{CONFIG}Arrow.png");
            Config_KeyAssign = TxC($"{CONFIG}KeyAssign.png");
            Config_Font = TxC($"{CONFIG}Font.png");
            Config_Font_Bold = TxC($"{CONFIG}Font_Bold.png");
            Config_Enum_Song = TxC($"{CONFIG}Enum_Song.png");
            #endregion

            #region 3_選曲画面
            SongSelect_Background = TxC($"{SONGSELECT}Background.png");
            SongSelect_Header = TxC($"{SONGSELECT}Header.png");
            SongSelect_Footer = TxC($"{SONGSELECT}Footer.png");
            SongSelect_Difficulty = TxC($"{SONGSELECT}Difficulty.png");
            SongSelect_Auto = TxC($"{SONGSELECT}Auto.png");
            SongSelect_Level = TxC($"{SONGSELECT}Level.png");
            SongSelect_Branch = TxC($"{SONGSELECT}Branch.png");
            SongSelect_Branch_Text = TxC($"{SONGSELECT}Branch_Text.png");
            SongSelect_Bar_Center = TxC($"{SONGSELECT}Bar_Center.png");
            SongSelect_Frame_Score = TxC($"{SONGSELECT}Frame_Score.png");
            SongSelect_Frame_BackBox = TxC($"{SONGSELECT}Frame_BackBox.png");
            SongSelect_Frame_Random = TxC($"{SONGSELECT}Frame_Random.png");
            SongSelect_Score_Select = TxC($"{SONGSELECT}Score_Select.png");
            // SongSelect_Frame_Dani = TxC($"{SONGSELECT}Frame_Dani.png");
            SongSelect_GenreText = TxC($"{SONGSELECT}GenreText.png");
            SongSelect_Cursor_Left = TxC($"{SONGSELECT}Cursor_Left.png");
            SongSelect_Cursor_Right = TxC($"{SONGSELECT}Cursor_Right.png");
            SongSelect_Bar_Genre = TxC(9, $"{SONGSELECT}Bar_Genre_{{0}}.png");
            SongSelect_Frame_Box = TxC(9, $"{SONGSELECT}Frame_Box_{{0}}.png");
            SongSelect_ScoreWindow = TxC((int) Difficulty.Total, $"{SONGSELECT}ScoreWindow_{{0}}.png");
            SongSelect_GenreBack = TxC(9, $"{SONGSELECT}GenreBackground_{{0}}.png");
            SongSelect_ScoreWindow_Text = TxC($"{SONGSELECT}ScoreWindow_Text.png");
            SongSelect_Rating = TxC($"{SONGSELECT}Rating.png");
            #endregion

            #region 4_読み込み画面
            SongLoading_Plate = TxC($"{SONGLOADING}Plate.png");
            SongLoading_FadeIn = TxC($"{SONGLOADING}FadeIn.png");
            SongLoading_FadeOut = TxC($"{SONGLOADING}FadeOut.png");
            #endregion

            #region 5_演奏画面
            #region 共通
            Notes = TxC($"{GAME}Notes.png");
            Judge_Frame = TxC($"{GAME}Notes.png");
            SENotes = TxC($"{GAME}SENotes.png");
            Notes_Arm = TxC($"{GAME}Notes_Arm.png");
            Judge = TxC($"{GAME}Judge.png");

            Judge_Meter = TxC($"{GAME}Judge_Meter.png");
            Bar = TxC($"{GAME}Bar.png");
            Bar_Branch = TxC($"{GAME}Bar_Branch.png");

            #endregion
            #region キャラクター

            (TJAPlayer3.Skin.Game_Chara_Ptn_Normal, Chara_Normal) = TxCFolder($@"{GAME}{CHARA}Normal\");
            (TJAPlayer3.Skin.Game_Chara_Ptn_Clear, Chara_Normal_Cleared) = TxCFolder($@"{GAME}{CHARA}Clear\");
            (_, Chara_Normal_Maxed) = TxCFolder($@"{GAME}{CHARA}Clear_Max\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_GoGo, Chara_GoGoTime) = TxCFolder($@"{GAME}{CHARA}GoGo\");
            (_, Chara_GoGoTime_Maxed) = TxCFolder($@"{GAME}{CHARA}GoGo_Max\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_10combo, Chara_10Combo) = TxCFolder($@"{GAME}{CHARA}10combo\");
            (TJAPlayer3.Skin.Game_Chara_Ptn_10combo_Max, Chara_10Combo_Maxed) = TxCFolder($@"{GAME}{CHARA}10combo_Max\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart, Chara_GoGoStart) = TxCFolder($@"{GAME}{CHARA}GoGoStart\");
            (TJAPlayer3.Skin.Game_Chara_Ptn_GoGoStart_Max, Chara_GoGoStart_Maxed) = TxCFolder($@"{GAME}{CHARA}GoGoStart_Max\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_ClearIn, Chara_Become_Cleared) = TxCFolder($@"{GAME}{CHARA}ClearIn\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_SoulIn, Chara_Become_Maxed) = TxCFolder($@"{GAME}{CHARA}SoulIn\");

            (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Breaking, Chara_Balloon_Breaking) = TxCFolder($@"{GAME}{CHARA}Balloon_Breaking\");
            (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Broke, Chara_Balloon_Broke) = TxCFolder($@"{GAME}{CHARA}Balloon_Broke\");
            (TJAPlayer3.Skin.Game_Chara_Ptn_Balloon_Miss, Chara_Balloon_Miss) = TxCFolder($@"{GAME}{CHARA}Balloon_Miss\");

            #endregion
            #region 踊り子

            var skinGameDancerPtn = TJAPlayer3.t連番画像の枚数を数える(CSkin.Path($@"{BASE}{GAME}{DANCER}1\"));
            if (skinGameDancerPtn != 0)
            {
                Dancer = new CTexture[5][];
                for (int i = 0; i < Dancer.Length; i++)
                {
                    Dancer[i] = new CTexture[skinGameDancerPtn];
                    for (int p = 0; p < skinGameDancerPtn; p++)
                    {
                        Dancer[i][p] = TxC($@"{GAME}{DANCER}{(i + 1)}\{p}.png");
                    }
                }
            }
            TJAPlayer3.Skin.Game_Dancer_Ptn = skinGameDancerPtn;

            #endregion
            #region モブ

            (TJAPlayer3.Skin.Game_Mob_Ptn, Mob) = TxCFolder($"{GAME}{MOB}");

            #endregion
            #region フッター
            Mob_Footer = TxC($"{GAME}{FOOTER}0.png");
            #endregion
            #region 背景
            Background = TxC($@"{GAME}{BACKGROUND}0\Background.png");
            Background_Up = TxC(2, $@"{GAME}{BACKGROUND}0\{{0}}P_Up.png", 1);
            Background_Up_Clear = TxC(2, $@"{GAME}{BACKGROUND}0\{{0}}P_Up_Clear.png", 1);
            Background_Down = TxC($@"{GAME}{BACKGROUND}0\Down.png");
            Background_Down_Clear = TxC($@"{GAME}{BACKGROUND}0\Down_Clear.png");
            Background_Down_Scroll = TxC($@"{GAME}{BACKGROUND}0\Down_Scroll.png");

            #endregion
            #region 太鼓
            Taiko_Background = TxC(2, $"{GAME}{TAIKO}{{0}}P_Background.png", 1);

            Taiko_Frame = TxC(2, $"{GAME}{TAIKO}{{0}}P_Frame.png", 1);

            Taiko_PlayerNumber = TxC(2, $"{GAME}{TAIKO}{{0}}P_PlayerNumber.png", 1);

            Taiko_NamePlate = TxC(2, $"{GAME}{TAIKO}{{0}}P_NamePlate.png", 1);

            Taiko_Base = TxC($"{GAME}{TAIKO}Base.png");
            Taiko_Don_Left = TxC($"{GAME}{TAIKO}Don.png");
            Taiko_Don_Right = TxC($"{GAME}{TAIKO}Don.png");
            Taiko_Ka_Left = TxC($"{GAME}{TAIKO}Ka.png");
            Taiko_Ka_Right = TxC($"{GAME}{TAIKO}Ka.png");
            Taiko_LevelUp = TxC($"{GAME}{TAIKO}LevelUp.png");
            Taiko_LevelDown = TxC($"{GAME}{TAIKO}LevelDown.png");

            Course_Symbol = TxC($"{GAME}{COURSESYMBOL}{{0}}.png", "Easy", "Normal", "Hard", "Oni", "Edit", "Tower", "Dan", "Shin");

            Taiko_Score = TxC($"{GAME}{TAIKO}Score{{0}}.png", "", "_1P", "_2P");

            Taiko_Combo = TxC($"{GAME}{TAIKO}Combo{{0}}.png", "", "_Big");
            Taiko_Combo_Effect = TxC($"{GAME}{TAIKO}Combo_Effect.png");
            Taiko_Combo_Text = TxC($"{GAME}{TAIKO}Combo_Text.png");
            #endregion
            #region ゲージ

            Gauge = TxC(2, $"{GAME}{GAUGE}{{0}}P.png", 1);
            Gauge_Hard = TxC(2, $"{GAME}{GAUGE}{{0}}P_Hard.png", 1);
            Gauge_ExHard = TxC(2, $"{GAME}{GAUGE}{{0}}P_ExHard.png", 1);
            Gauge_Base = TxC(2, $"{GAME}{GAUGE}{{0}}P_Base.png", 1);
            Gauge_Base_Hard = TxC(2, $"{GAME}{GAUGE}{{0}}P_Base_Hard.png", 1);
            Gauge_Base_ExHard = TxC(2, $"{GAME}{GAUGE}{{0}}P_Base_ExHard.png", 1);
            Gauge_Line = TxC(2, $"{GAME}{GAUGE}{{0}}P_Line.png", 1);
            Gauge_Line_Hard = TxC(2, $"{GAME}{GAUGE}{{0}}P_Line_Hard.png", 1);

            (TJAPlayer3.Skin.Game_Gauge_Rainbow_Ptn, Gauge_Rainbow) = TxCFolder($@"{GAME}{GAUGE}Rainbow\");

            Gauge_Soul = TxC($"{GAME}{GAUGE}Soul.png");
            Gauge_Soul_Fire = TxC($"{GAME}{GAUGE}Fire.png");

            Gauge_Soul_Explosion = TxC(2, $"{GAME}{GAUGE}{{0}}P_Explosion.png", 1);

            #endregion
            #region 吹き出し
            Balloon_Combo = TxC(2, $"{GAME}{BALLOON}Combo_{{0}}P.png", 1);
            Balloon_Roll = TxC($"{GAME}{BALLOON}Roll.png");
            Balloon_Balloon = TxC($"{GAME}{BALLOON}Balloon.png");
            Balloon_Number_Roll = TxC($"{GAME}{BALLOON}Number_Roll.png");
            Balloon_Number_Combo = TxC($"{GAME}{BALLOON}Number_Combo.png");
            Balloon_Breaking = TxC(6, $"{GAME}{BALLOON}Breaking_{{0}}.png");

            #endregion
            #region エフェクト
            Effects_Hit_Explosion = TxCAf($@"{GAME}{EFFECTS}Hit\Explosion.png");
            if (Effects_Hit_Explosion != null) Effects_Hit_Explosion.b加算合成 = TJAPlayer3.Skin.Game_Effect_HitExplosion_AddBlend;
            Effects_Hit_Explosion_Big = TxC($@"{GAME}{EFFECTS}Hit\Explosion_Big.png");
            if (Effects_Hit_Explosion_Big != null) Effects_Hit_Explosion_Big.b加算合成 = TJAPlayer3.Skin.Game_Effect_HitExplosionBig_AddBlend;
            Effects_Hit_FireWorks = TxC($@"{GAME}{EFFECTS}Hit\FireWorks.png");
            if (Effects_Hit_FireWorks != null) Effects_Hit_FireWorks.b加算合成 = TJAPlayer3.Skin.Game_Effect_FireWorks_AddBlend;


            Effects_Fire = TxC($"{GAME}{EFFECTS}Fire.png");
            if (Effects_Fire != null) Effects_Fire.b加算合成 = TJAPlayer3.Skin.Game_Effect_Fire_AddBlend;

            Effects_Rainbow = TxC($"{GAME}{EFFECTS}Rainbow.png");

            Effects_GoGoSplash = TxC($"{GAME}{EFFECTS}GoGoSplash.png");
            if (Effects_GoGoSplash != null) Effects_GoGoSplash.b加算合成 = TJAPlayer3.Skin.Game_Effect_GoGoSplash_AddBlend;

            Effects_Hit_Good = TxC(15, $@"{GAME}{EFFECTS}Hit\Good\{{0}}.png");
            Effects_Hit_Good_Big = TxC(15, $@"{GAME}{EFFECTS}Hit\Good_Big\{{0}}.png");
            Effects_Hit_Great = TxC(15, $@"{GAME}{EFFECTS}Hit\Great\{{0}}.png");
            Effects_Hit_Great_Big = TxC(15, $@"{GAME}{EFFECTS}Hit\Great_Big\{{0}}.png");

            (TJAPlayer3.Skin.Game_Effect_Roll_Ptn, Effects_Roll) = TxCFolder($@"{GAME}{EFFECTS}Roll\");

            #endregion
            #region レーン
            var lanes = new[] { "Normal", "Expert", "Master" };
            Lane_Base = TxC($"{GAME}{LANE}Base_{{0}}.png", lanes);
            Lane_Text = TxC($"{GAME}{LANE}Text_{{0}}.png", lanes);

            Lane_Red = TxC($"{GAME}{LANE}Red.png");
            Lane_Blue = TxC($"{GAME}{LANE}Blue.png");
            Lane_Yellow = TxC($"{GAME}{LANE}Yellow.png");
            Lane_Background_Main = TxC($"{GAME}{LANE}Background_Main.png");
            Lane_Background_Sub = TxC($"{GAME}{LANE}Background_Sub.png");
            Lane_Background_GoGo = TxC($"{GAME}{LANE}Background_GoGo.png");

            #endregion
            #region 終了演出

            End_Clear_L = TxC(5, $"{GAME}{END}Clear_L_{{0}}.png");
            End_Clear_R = TxC(5, $"{GAME}{END}Clear_R_{{0}}.png");

            End_Clear_Text = TxC($"{GAME}{END}Clear_Text.png");
            End_Clear_Text_Effect = TxC($"{GAME}{END}Clear_Text_Effect.png");
            if (End_Clear_Text_Effect != null) End_Clear_Text_Effect.b加算合成 = true;
            #endregion
            #region ゲームモード
            GameMode_Timer_Tick = TxC($"{GAME}{GAMEMODE}Timer_Tick.png");
            GameMode_Timer_Frame = TxC($"{GAME}{GAMEMODE}Timer_Frame.png");
            #endregion
            #region ステージ失敗
            Failed_Game = TxC($"{GAME}{FAILED}Game.png");
            Failed_Stage = TxC($"{GAME}{FAILED}Stage.png");
            #endregion
            #region ランナー
            Runner = TxC($"{GAME}{RUNNER}0.png");
            #endregion
            #region DanC
            DanC_Background = TxC($"{GAME}{DANC}Background.png");

            DanC_Gauge = TxC($"{GAME}{DANC}Gauge_{{0}}.png", "Normal", "Reach", "Clear", "Flush");

            DanC_Base = TxC($"{GAME}{DANC}Base.png");
            DanC_Failed = TxC($"{GAME}{DANC}Failed.png");
            DanC_Number = TxC($"{GAME}{DANC}Number.png");
            DanC_ExamType = TxC($"{GAME}{DANC}ExamType.png");
            DanC_ExamRange = TxC($"{GAME}{DANC}ExamRange.png");
            DanC_ExamUnit = TxC($"{GAME}{DANC}ExamUnit.png");
            DanC_Screen = TxC($"{GAME}{DANC}Screen.png");
            #endregion
            #region PuichiChara
            PuchiChara = TxC($"{GAME}{PUCHICHARA}0.png");
            #endregion
            #endregion

            #region 6_結果発表
            Result_Background = TxC($"{RESULT}Background.png");
            Result_FadeIn = TxC($"{RESULT}FadeIn.png");
            Result_Gauge = TxC($"{RESULT}Gauge.png");
            Result_Gauge_Base = TxC($"{RESULT}Gauge_Base.png");
            Result_Gauge_Hard = TxC($"{RESULT}Gauge_Hard.png");
            Result_Gauge_ExHard = TxC($"{RESULT}Gauge_ExHard.png");
            Result_Gauge_Base_Hard = TxC($"{RESULT}Gauge_Base_Hard.png");
            Result_Gauge_Base_ExHard = TxC($"{RESULT}Gauge_Base_ExHard.png");
            Result_Judge = TxC($"{RESULT}Judge.png");
            Result_Header = TxC($"{RESULT}Header.png");
            Result_Number = TxC($"{RESULT}Number.png");
            Result_Panel = TxC($"{RESULT}Panel.png");
            Result_Score_Text = TxC($"{RESULT}Score_Text.png");
            Result_Score_Number = TxC($"{RESULT}Score_Number.png");
            Result_Dan = TxC($"{RESULT}Dan.png");
            #endregion

            #region 7_終了画面
            Exit_Background = TxC($"{EXIT}Background.png");
            #endregion

        }

        public void Dispose()
        {
            _genreTexturesByFileNameWithoutExtension.Clear();

            foreach (var texture in _trackedTextures)
            {
                texture.Dispose();
            }
            _trackedTextures.Clear();
        }

        #region 共通
        public CTexture Tile_Black,
            Tile_White,
            Menu_Title,
            Menu_Highlight,
            Enum_Song,
            Scanning_Loudness,
            Overlay;
        public CTexture[] NamePlate;
        #endregion
        #region 1_タイトル画面
        public CTexture Title_Background,
            Title_Menu;
        #endregion

        #region 2_コンフィグ画面
        public CTexture Config_Background,
            Config_Cursor,
            Config_ItemBox,
            Config_Arrow,
            Config_KeyAssign,
            Config_Font,
            Config_Font_Bold,
            Config_Enum_Song;
        #endregion

        #region 3_選曲画面
        public CTexture SongSelect_Background,
            SongSelect_Header,
            SongSelect_Footer,
            SongSelect_Difficulty,
            SongSelect_Auto,
            SongSelect_Level,
            SongSelect_Branch,
            SongSelect_Branch_Text,
            SongSelect_Frame_Score,
            SongSelect_Frame_BackBox,
            SongSelect_Frame_Random,
            SongSelect_Score_Select,
            SongSelect_Bar_Center,
            SongSelect_GenreText,
            SongSelect_Cursor_Left,
            SongSelect_Cursor_Right,
            SongSelect_ScoreWindow_Text,
            SongSelect_Rating;

        public CTexture[] SongSelect_GenreBack;
        public CTexture[] SongSelect_ScoreWindow;
        public CTexture[] SongSelect_Bar_Genre;
        public CTexture[] SongSelect_Frame_Box;

        #endregion

        #region 4_読み込み画面
        public CTexture SongLoading_Plate,
            SongLoading_FadeIn,
            SongLoading_FadeOut;
        #endregion

        #region 5_演奏画面
        #region 共通
        public CTexture Notes,
            Judge_Frame,
            SENotes,
            Notes_Arm,
            Judge;
        public CTexture Judge_Meter,
            Bar,
            Bar_Branch;
        #endregion
        #region キャラクター
        public CTexture[] Chara_Normal,
            Chara_Normal_Cleared,
            Chara_Normal_Maxed,
            Chara_GoGoTime,
            Chara_GoGoTime_Maxed,
            Chara_10Combo,
            Chara_10Combo_Maxed,
            Chara_GoGoStart,
            Chara_GoGoStart_Maxed,
            Chara_Become_Cleared,
            Chara_Become_Maxed,
            Chara_Balloon_Breaking,
            Chara_Balloon_Broke,
            Chara_Balloon_Miss;
        #endregion
        #region 踊り子
        public CTexture[][] Dancer;
        #endregion
        #region モブ
        public CTexture[] Mob;
        public CTexture Mob_Footer;
        #endregion
        #region 背景
        public CTexture Background,
            Background_Down,
            Background_Down_Clear,
            Background_Down_Scroll;
        public CTexture[] Background_Up;
        public CTexture[] Background_Up_Clear;

        #endregion
        #region 太鼓

        public CTexture[] Taiko_Frame; // MTaiko下敷き
        public CTexture[] Taiko_Background;
        public CTexture Taiko_Base,
            Taiko_Don_Left,
            Taiko_Don_Right,
            Taiko_Ka_Left,
            Taiko_Ka_Right,
            Taiko_LevelUp,
            Taiko_LevelDown,
            Taiko_Combo_Effect,
            Taiko_Combo_Text;

        public CTexture[] Course_Symbol;
        public CTexture[] Taiko_PlayerNumber;
        public CTexture[] Taiko_NamePlate; // ネームプレート
        public CTexture[] Taiko_Score;
        public CTexture[] Taiko_Combo;
        #endregion
        #region ゲージ
        public CTexture[] Gauge;
        public CTexture[] Gauge_Hard;
        public CTexture[] Gauge_ExHard;
        public CTexture[] Gauge_Base;
        public CTexture[] Gauge_Base_Hard;
        public CTexture[] Gauge_Base_ExHard;
        public CTexture[] Gauge_Line;
        public CTexture[] Gauge_Line_Hard;
        public CTexture[] Gauge_Rainbow;
        public CTexture[] Gauge_Soul_Explosion;

        public CTexture Gauge_Soul,
            Gauge_Soul_Fire;
        #endregion
        #region 吹き出し
        public CTexture[] Balloon_Combo;
        public CTexture Balloon_Roll,
            Balloon_Balloon,
            Balloon_Number_Roll,
            Balloon_Number_Combo/*,*/
                                /*Balloon_Broken*/;
        public CTexture[] Balloon_Breaking;
        #endregion
        #region エフェクト
        public CTexture Effects_Hit_Explosion,
            Effects_Hit_Explosion_Big,
            Effects_Fire,
            Effects_Rainbow,
            Effects_GoGoSplash,
            Effects_Hit_FireWorks;

        public CTexture[] Effects_Hit_Good;
        public CTexture[] Effects_Hit_Good_Big;
        public CTexture[] Effects_Hit_Great;
        public CTexture[] Effects_Hit_Great_Big;

        public CTexture[] Effects_Roll;
        #endregion
        #region レーン
        public CTexture[] Lane_Base;
        public CTexture[] Lane_Text;

        public CTexture Lane_Red,
            Lane_Blue,
            Lane_Yellow;
        public CTexture Lane_Background_Main,
            Lane_Background_Sub,
            Lane_Background_GoGo;
        #endregion
        #region 終了演出
        public CTexture[] End_Clear_L;
        public CTexture[] End_Clear_R;

        public CTexture End_Clear_Text,
            End_Clear_Text_Effect;
        #endregion
        #region ゲームモード
        public CTexture GameMode_Timer_Frame,
            GameMode_Timer_Tick;
        #endregion
        #region ステージ失敗
        public CTexture Failed_Game,
            Failed_Stage;
        #endregion
        #region ランナー
        public CTexture Runner;
        #endregion
        #region DanC
        public CTexture DanC_Background;
        public CTexture[] DanC_Gauge;
        public CTexture DanC_Base;
        public CTexture DanC_Failed;
        public CTexture DanC_Number,
            DanC_ExamType,
            DanC_ExamRange,
            DanC_ExamUnit;
        public CTexture DanC_Screen;
        #endregion
        #region PuchiChara
        public CTexture PuchiChara;
        #endregion
        #endregion

        #region 6_結果発表
        public CTexture Result_Background,
            Result_FadeIn,
            Result_Gauge,
            Result_Gauge_Hard,
            Result_Gauge_ExHard,
            Result_Gauge_Base,
            Result_Gauge_Base_Hard,
            Result_Gauge_Base_ExHard,
            Result_Judge,
            Result_Header,
            Result_Number,
            Result_Panel,
            Result_Score_Text,
            Result_Score_Number,
            Result_Dan;
        #endregion

        #region 7_終了画面
        public CTexture Exit_Background/* , */
                                       /*Exit_Text */;
        #endregion

    }
}
