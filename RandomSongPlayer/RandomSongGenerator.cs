using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BeatSaverSharp;

namespace RandomSongPlayer
{
    internal static class RandomSongGenerator
    {
        /// <summary>
        /// Pulls a random beatmap from BeatSaver using a default filter
        /// (uses settings in mod)
        /// </summary>
        /// <returns>A beatmap that has been chosen randomly, or null</returns>
        internal static async Task<Beatmap> GenerateRandomKey()
        {
            Logger.log.Debug(Environment.StackTrace); // TODO remove debug
            return await GenerateRandomKey(Filter);
        }
        /// <summary>
        /// Pulls a random beatmap from BeatSaver that passes the filter
        /// </summary>
        /// <param name="filter">A filter to pass the map through</param>
        /// <returns>A beatmap that has been chosen randomly, or null</returns>
        internal static async Task<Beatmap> GenerateRandomKey(Func<Beatmap, bool> mapfilter)
        {
            int tries = 0;
            int maxTries = Plugin.config.MaxRetries;
            string randomKey = null;
            Beatmap mapData = null;

            Logger.log.Info("Searching for random beatmap");

            // Look for the latest key on the Beatsaver API
            Page latestMaps = await Plugin.beatsaverClient.Latest();
            string latestKey = latestMaps.Docs[0].Key;
            int keyAsDecimal = int.Parse(latestKey, System.Globalization.NumberStyles.HexNumber);

            // Randomize the key and download the map
            do
            {
                int randomNumber = Plugin.rnd.Next(0, keyAsDecimal + 1);
                randomKey = randomNumber.ToString("x");
                mapData = await UpdateMapData(randomKey);
                if (mapfilter != null && mapfilter(mapData))
                {
                    return mapData;
                }
                tries++;
            } while (tries < maxTries);
            return null;
        }

        /// <summary>
        /// Tries to get a map with the specified key
        /// </summary>
        /// <param name="randomKey">Key whose beatmap to get</param>
        /// <returns>A beatmap or null</returns>
        private static async Task<Beatmap> UpdateMapData(string randomKey)
        {
            try
            {
                Beatmap mapData = await Plugin.beatsaverClient.Key(randomKey);
                if (!(mapData is null))
                {
                    Logger.log.Info("Found map " + randomKey + ": " + mapData.Metadata.SongAuthorName + " - " + mapData.Metadata.SongName + " by " + mapData.Metadata.LevelAuthorName);
                }
                return mapData;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Logger.log.Info("Failed to download map with key '" + randomKey + "'. Map was most likely deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /// <summary>
        /// Will check if the beatmap satisfies all the filters
        /// specified in the config file.
        /// If any filters cannot be checked because of null values,
        /// the beatmap will be assumed to pass those filters
        ///
        /// Also checks to be sure we aren't downloading a map we already
        /// have saved
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns>True if the beatmap satisfies the filters</returns>
        internal static bool Filter(Beatmap mapData)
        {
            //TODO remove debug
            Logger.log.Info("Trying with " + mapData.ID);

            FilterConfig config = Plugin.config.Filter;
            if (config == null)
            {
                return true;
            }
            Metadata meta = mapData.Metadata;

            // Check all difficulties
            Difficulties difficulties = meta.Difficulties;
            if (
                config.Easy && !difficulties.Easy
                || config.Normal && !difficulties.Normal
                || config.Hard && !difficulties.Hard
                || config.Expert && !difficulties.Expert
                || config.ExpertPlus && !difficulties.ExpertPlus
            )
            {
                return false;
            }
            //Check BPM and length
            if (
                meta.BPM < config.MinBPM || meta.BPM > config.MaxBPM
                || meta.Duration < config.MinLength || meta.Duration > config.MaxLength
            )
            {
                return false;
            }
            //Check rating and download count
            Stats stat = mapData.Stats;
            if (stat.Rating < config.MinRating || stat.Downloads < config.MinDownloads)
            {
                return false;
            }

            //Ensure the song isn't yet downloaded
            if (SongCore.Collections.RetrieveExtraSongData(mapData.ID) != null)
            {
                return false;
            }

            return true;
        }
    }
}
