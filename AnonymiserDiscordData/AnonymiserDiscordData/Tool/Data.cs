using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Rygone.Tool;

namespace AnonymiserDiscordData.Tool
{
    public static class Data
    {
        public delegate void ActionError(string error);
        public delegate void SetMax(int max);
        public delegate void SetValue(int value);
        public delegate void SetLabel(string value);

        private static Dictionary<string, string> ServerIDs;
        private static Dictionary<string, string> ChannelIDs;

        private static JObject JServerIDs;
        private static JObject JChannelIDs;

        public static string Get(string key, Dictionary<string, string> IDs, JObject JIDs, bool HideIDs, bool HideNames, string basse)
        {
            if (!IDs.ContainsKey(key))
            {
                if (HideIDs)
                {
                    if (HideNames || !JIDs.ContainsKey(key))
                        return IDs[key] = basse + IDs.Count;
                    else
                        return IDs[key] = (string)JIDs[key];
                }
                else
                    return IDs[key] = key;
            }
            return IDs[key];
        }
        public static string GetChannel(string key, bool HideIDs, bool HideNames)
            => Get(key, ChannelIDs, JChannelIDs, HideIDs, HideNames, "Channel_");
        public static string GetServer(string key, bool HideIDs, bool HideNames)
            => Get(key, ServerIDs, JServerIDs, HideIDs, HideNames, "Server_");



        public static void Start(
            string Path,
            Form form,
            SetMax Max,
            SetValue Value,
            SetLabel Label,
            Action End,
            ActionError Error,
            bool HideMessages,
            bool DeleteMessages,
            bool HideNicknames,
            bool HideServerIDs,
            bool HideServerNames,
            bool HideChannelIDs,
            bool HideChannelNames,
            bool HideApplication,
            bool DeleteApplication,
            bool HideOS,
            bool HideIPs,
            bool HideLocations,
            bool DeleteActivities
            )
        {
            Task t = new Task(() =>
            {
                form.Invoke(Max, 102);
                form.Invoke(Value, 0);
                int start = 0;
                int max = 0;

                #region UnZip
                max = 5;
                form.Invoke(Label, "UnZip");
                if (!Ziper.UnZip(Path))
                {
                    Error("UnZipFailed");
                    return;
                }
                form.Invoke(Value, start += max);
                #endregion


                #region IDs
                max = 5;
                form.Invoke(Label, "IDs");
                {
                    ServerIDs = new Dictionary<string, string>();
                    ChannelIDs = new Dictionary<string, string>();
                    string channel = $@"{Ziper.TempPath}\messages\index.json";
                    string servers = $@"{Ziper.TempPath}\servers\index.json";

                    if (File.Exists(servers))
                        JServerIDs = JObject.Parse(File.ReadAllText(servers));
                    if (File.Exists(channel))
                        JChannelIDs = JObject.Parse(File.ReadAllText(channel));
                }
                form.Invoke(Value, start += max);
                #endregion


                #region Messages
                max = 30;
                form.Invoke(Label, "Messages");
                if (DeleteMessages)
                {
                    string messages = $@"{Ziper.TempPath}\messages";
                    if (Directory.Exists(messages))
                        foreach (string directory in Directory.GetDirectories(messages))
                            Directory.Delete(directory, true);
                }
                else
                {
                    if (HideMessages)
                    {
                        string messages = $@"{Ziper.TempPath}\messages";
                        static string[] fl(IReadOnlyDictionary<string, string> line, int nb) => new string[] { line.ContainsKey("Timestamp") ? line["Timestamp"] : "Timestamp" };
                        if (Directory.Exists(messages))
                        {
                            List<Task> tasks = new List<Task>();
                            string[] directories = Directory.GetDirectories(messages);
                            foreach (string directory in directories)
                                tasks.Add(Rygone.Tool.Editor.CSVEditor.Edit($@"{directory}\messages.csv", fl));
                            int i = 0;
                            foreach (Task task in tasks)
                            {
                                task.Wait();
                                form.Invoke(Value, start + ((max * ++i) / tasks.Count));
                            }
                        }
                    }
                }
                form.Invoke(Value, start += max);
                #endregion

                #region user.json
                max = 5;
                form.Invoke(Label, "user.json");
                {
                    string userjs = $@"{Ziper.TempPath}\account\user.json";
                    if (File.Exists(userjs))
                    {
                        JObject js = JObject.Parse(File.ReadAllText(userjs));
                        if (HideNicknames)
                        {
                            js.Remove("id");
                            js.Remove("username");
                            js.Remove("email");
                            js.Remove("avatar_hash");
                            foreach (JObject relation in (JArray)js["relationships"])
                            {
                                relation.Remove("id");
                                relation.Remove("nickname");
                                JObject user = (JObject)relation["user"];
                                user.Remove("id");
                                user.Remove("username");
                                user.Remove("avatar");
                            }
                            js.Remove("payments");
                            js.Remove("external_friends_lists");
                            js.Remove("friend_suggestions");
                        }
                        if (HideIPs)
                        {
                            js.Remove("ip");
                        }
                        if (HideServerNames)
                            js.Remove("guild_settings");
                        else
                            foreach (JObject guild_setting in (JArray)js["guild_settings"])
                            {
                                if (HideServerIDs)
                                    guild_setting.Remove("guild_id");
                                if (HideChannelNames)
                                    guild_setting.Remove("channel_overrides");
                                else if (HideChannelIDs)
                                    foreach (JObject channel_overrides in (JArray)guild_setting["channel_overrides"])
                                        channel_overrides.Remove("channel_id");
                            }
                        if (HideServerIDs)
                        {
                            JObject settings = (JObject)js["settings"];
                            settings.Remove("guild_positions");
                            settings.Remove("guild_folders");
                        }
                        if (DeleteApplication)
                        {
                            js.Remove("library_applications");
                            js.Remove("entitlements");
                            js.Remove("user_activity_application_statistics");
                        }
                        else
                            foreach (JObject user_activity_application_statistic in (JArray)js["user_activity_application_statistics"])
                            {
                                user_activity_application_statistic.Remove("application_id");
                            }
                        if (HideMessages)
                            js.Remove("notes");
                        File.WriteAllText(userjs, js.ToString());
                    }
                }
                form.Invoke(Value, start += max);
                #endregion

                #region NickName
                max = 5;
                form.Invoke(Label, "NickName");
                if (HideNicknames)
                {
                    string avatar = $@"{Ziper.TempPath}\account\avatar.png";
                    if (File.Exists(avatar))
                        File.Delete(avatar);
                }
                form.Invoke(Value, start += max);
                #endregion

                #region Activity
                max = 50;
                form.Invoke(Label, "Activity");
                {
                    if (DeleteActivities)
                    {
                        string messages = $@"{Ziper.TempPath}\activity";
                        if (Directory.Exists(messages))
                            Directory.Delete(messages, true);
                    }
                    else if (HideIPs || HideOS || HideLocations)
                    {
                        string messages = $@"{Ziper.TempPath}\activity";
                        if (Directory.Exists(messages))
                        {
                            List<Task> tasks = new List<Task>();

                            List<string> list = new List<string>(new string[] {
                                "event_type",
                                "event_source",
                                "user_id",
                                "domain",
                                "ip",
                                "chosen_locale",
                                "detected_locale",
                                "user_is_authenticated",
                                "browser",
                                "device",
                                "os",
                                "os_version",
                                "os_arch",
                                "city",
                                "country_code",
                                "region_code",
                                "time_zone",
                                "isp",
                                "message_id",
                                "channel",
                                "channel_type",
                                "is_friend",
                                "private",
                                "server",
                                "length",
                                "word_count",
                                "mention_everyone",
                                "emoji_unicode",
                                "emoji_custom_external",
                                "emoji_managed",
                                "emoji_managed_external",
                                "emoji_animated",
                                "emoji_only",
                                "has_spoiler",
                                "probably_has_markdown",
                                "timestamp",
                            });
                            if (HideMessages || DeleteMessages) list.Remove("message_id");
                            if (HideChannelIDs)
                            {
                                list.Remove("channel");
                                list.Remove("channel_type");
                            }
                            if (HideServerIDs) list.Remove("server");
                            if (HideNicknames) list.Remove("user_id");
                            if (HideIPs)
                            {
                                list.Remove("ip");
                                list.Remove("isp");
                            }
                            if (HideOS)
                            {
                                list.Remove("browser");
                                list.Remove("device");
                                list.Remove("os");
                                list.Remove("os_version");
                                list.Remove("os_arch");
                            }
                            if (HideLocations)
                            {
                                list.Remove("city");
                                list.Remove("country_code");
                                list.Remove("region_code");
                                list.Remove("time_zone");
                                list.Remove("isp");
                            }
                            string[] array = list.ToArray();
                            JObject fl(JObject line, int nb)
                            {
                                JObject res = new JObject();
                                foreach (string key in array)
                                    if (line.ContainsKey(key))
                                        res.Add(key, line[key]);
                                return res;
                            }
                            foreach (string directory in Directory.EnumerateDirectories(messages))
                                foreach (string file in Directory.EnumerateFiles(directory))
                                    tasks.Add(Rygone.Tool.Editor.JSEditor.Edit(file, fl));
                            int i = 0;
                            foreach (Task task in tasks)
                            {
                                task.Wait();
                                form.Invoke(Value, start + ((max * ++i) / tasks.Count));
                            }
                        }
                    }
                }
                form.Invoke(Value, start += max);
                #endregion

                #region Zip
                max = 5;
                form.Invoke(Label, "Zip");
                Ziper.Zip($"{Path[0..^4]}-Anonymiser.zip");
                Directory.Delete(Ziper.TempPath, true);
                form.Invoke(Value, start += max);
                #endregion

                /*
                #region
                max = 5;
                form.Invoke(Label, "");
                {
                    
                }
                form.Invoke(Value, start += max);
                #endregion
                */

                form.Invoke(End);
            });
            t.Start();
        }
    }
}