using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace RandomSongPlayer
{
    /// <summary>
    /// Stores all the configuration data for the mod
    /// </summary>
    internal class PluginConfig
    {
        /// <summary>
        /// Where to put the Random button on the playlist
        /// </summary>
        [NonNullable]
        public ButtonConfig Button { get; set; } = new ButtonConfig();
        /// <summary>
        /// Options to filter what songs are randomly picked
        /// </summary>
        [NonNullable]
        public FilterConfig Filter { get; set; } = new FilterConfig();

        public int MaxRetries { get; set; } = 20;

        /// <summary>
        /// Call this to save to disk.
        /// Signals BSIPA the config object has been changed.
        /// </summary>
        public virtual void Changed() { }
        /// <summary>
        /// this is called whenever the config file is reloaded from disk
        /// use it to tell all of your systems that something has changed
        ///
        /// this is called off of the main thread, and is not safe to interact
        /// with Unity in
        /// </summary>
        public virtual void OnReload() { }
    }

    /// <summary>
    /// Stores the position and size of a button
    /// </summary>
    internal class ButtonConfig
    {
        public float Width { get; set; } = 22f;
        public float Height { get; set; } = 6f;
        public float X { get; set; } = -68.5f;
        public float Y { get; set; } = -5.5f;
        public string Text { get; set; } = "RSP";
    }

    /// <summary>
    /// Stores all the configuration data as to what songs are randomly selected
    /// </summary>
    internal class FilterConfig
    {
        /// <summary>
        /// Does the song need to be ranked on ScoreSaber?
        /// </summary>
        //public bool Ranked { get; set; } = false;
        //Impossible. Needs ScoreSaberSharp updated a bit first.

        /// <summary>
        /// Require songs to have an easy difficulty map
        /// </summary>
        public bool Easy { get; set; } = false;
        /// <summary>
        /// Require songs to have a normal difficulty map
        /// </summary>
        public bool Normal { get; set; } = false;
        /// <summary>
        /// Require songs to have a hard difficulty map
        /// </summary>
        public bool Hard { get; set; } = false;
        /// <summary>
        /// Require songs to have an expert difficulty map
        /// </summary>
        public bool Expert { get; set; } = false;
        /// <summary>
        /// Require songs to have an expert+ difficulty map
        /// </summary>
        public bool ExpertPlus { get; set; } = false;

        /// <summary>
        /// The minimum rating reqired of a song to get
        /// </summary>
        public float MinRating { get; set; } = 0;
        /// <summary>
        /// The minimum download count of a song to get
        /// </summary>
        public float MinDownloads { get; set; } = 0;

        /// <summary>
        /// The minimum length of a song to get, in seconds
        /// </summary>
        public float MinLength { get; set; } = 30;
        // 30 seconds seems like a reasonable length, right?
        /// <summary>
        /// The maximum length of a song to get
        /// </summary>
        public float MaxLength { get; set; } = 3600;
        // If someone has any song longer than an hour, whyyyyyyy?

        /// <summary>
        /// The minimum bpm of a song to get
        /// </summary>
        public float MinBPM { get; set; } = 0;
        /// <summary>
        /// The maximum bpm of a song to get
        /// </summary>
        public float MaxBPM { get; set; } = 600;
        // Again, if you've reached the limit, why exactly?
        // That's 10 beats per second!
    }
}
