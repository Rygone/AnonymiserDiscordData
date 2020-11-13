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
        public delegate void Action(string value);
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
                try
                {
                    if (HideIDs)
                    {
                        if (HideNames || JIDs == null || !JIDs.ContainsKey(key))
                            return IDs[key] = basse + IDs.Count;
                        else
                            return IDs[key] = (string)JIDs[key];
                    }
                    else
                        return IDs[key] = key;
                }
                catch(Exception e)
                {
                    if (IDs.ContainsKey(key))
                        return IDs[key];
                    else
                        throw e;
                }
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
            Action Error,
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
                form.Invoke(Max, 114);
                form.Invoke(Value, 0);
                int start = 0;
                int max = 0;

                #region UnZip
                max = 5;
                form.Invoke(Label, "UnZip");
                if (!Ziper.UnZip(Path))
                {
                    form.Invoke(Error, "UnZipFailed");
                    return;
                }
                form.Invoke(Value, start += max);
                #endregion

                #region IDs
                max = 2;
                form.Invoke(Label, "IDs");
                try
                {
                    ServerIDs = new Dictionary<string, string>();
                    ChannelIDs = new Dictionary<string, string>();
                    string channel = $@"{Ziper.TempPath}\messages\index.json";
                    string servers = $@"{Ziper.TempPath}\servers\index.json";

                    JServerIDs = File.Exists(servers) ? JObject.Parse(File.ReadAllText(servers)) : null;
                    JChannelIDs = File.Exists(channel) ? JObject.Parse(File.ReadAllText(channel)) : null;
                }
                catch { form.Invoke(Error, "IDsFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region Messages Channel
                max = 35;
                form.Invoke(Label, "Messages");
                string messages = $@"{Ziper.TempPath}\messages";
                try
                {
                    if (Directory.Exists(messages))
                    {
                        string[] directories = Directory.GetDirectories(messages);
                        if (DeleteMessages)
                        {
                            foreach (string directory in Directory.GetDirectories(messages))
                                Directory.Delete(directory, true);

                        }
                        else if (HideMessages)
                        {

                            static string[] fl(IReadOnlyDictionary<string, string> line, int nb) => new string[] { line.ContainsKey("Timestamp") ? line["Timestamp"] : "Timestamp" };
                            List<Task> tasks = new List<Task>();
                            foreach (string directory in directories)
                                tasks.Add(Rygone.Tool.Editor.CSVEditor.Edit($@"{directory}\messages.csv", fl));
                            int i = 0;
                            foreach (Task task in tasks)
                            {
                                task.Wait();
                                form.Invoke(Value, start + (((max - 5) * ++i) / tasks.Count));
                            }
                        }
                        form.Invoke(Label, "Channel");
                        string file = $@"{messages}\index.json";
                        if ((HideChannelIDs || HideChannelNames) && File.Exists(file))
                        {
                            JObject res = new JObject();
                            foreach (string directory in directories)
                            {
                                string from = directory.Substring(messages.Length + 1);
                                string to = GetChannel(from, HideChannelIDs, HideChannelNames);
                                if (HideChannelIDs)
                                    Directory.Move(directory, $@"{messages}\{to}");
                                res[HideChannelIDs ? to : from] = to;
                            }
                            File.WriteAllText(file, res.ToString());
                        }
                    }
                }
                catch { form.Invoke(Error, "MessagesChannelFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region user.json
                max = 5;
                form.Invoke(Label, "user.json");
                try
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
                catch { form.Invoke(Error, "user.jsonFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region NickName
                max = 2;
                form.Invoke(Label, "NickName");
                try
                {
                    if (HideNicknames)
                    {
                        string avatar = $@"{Ziper.TempPath}\account\avatar.png";
                        if (File.Exists(avatar))
                            File.Delete(avatar);
                    }
                }
                catch { form.Invoke(Error, "NickNameFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region Activity
                max = 50;
                form.Invoke(Label, "Activity");
                try
                {
                    if (DeleteActivities)
                    {
                        string activity = $@"{Ziper.TempPath}\activity";
                        if (Directory.Exists(activity))
                            Directory.Delete(activity, true);
                    }
                    else if (HideIPs || HideOS || HideLocations)
                    {
                        string activity = $@"{Ziper.TempPath}\activity";
                        if (Directory.Exists(activity))
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
                            if (HideChannelNames) list.Remove("channel");
                            if (HideChannelIDs) list.Remove("channel_type"); 
                            if (HideServerIDs && HideServerNames) list.Remove("server");
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
                                if (HideChannelIDs && !HideChannelNames && res.ContainsKey("channel"))
                                    res["channel"] = GetChannel((string)res["channel"], HideChannelIDs, HideChannelNames);
                                if (HideServerIDs && !HideServerNames && res.ContainsKey("server"))
                                    res["server"] = GetServer((string)res["server"], HideServerIDs, HideServerNames);
                                return res;
                            }
                            foreach (string directory in Directory.EnumerateDirectories(activity))
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
                catch { form.Invoke(Error, "ActivityFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region Servers
                max = 5;
                form.Invoke(Label, "Servers");
                try
                {
                    string servers = $@"{Ziper.TempPath}\servers";
                    List<string> serverIDs = new List<string>();
                    if (Directory.Exists(servers))
                    {
                        string[] directories = Directory.GetDirectories(servers);
                        foreach (string directory in directories)
                        {
                            if (HideNicknames)
                            {
                                string audit_log = $@"{directory}\audit-log.json";
                                if (File.Exists(audit_log))
                                    File.Delete(audit_log);
                            }
                            serverIDs.Add(directory.Substring(servers.Length + 1));
                        }
                        string file = $@"{servers}\index.json";
                        if ((HideServerIDs || HideServerNames) && File.Exists(file))
                        {
                            JObject res = new JObject();
                            foreach (string directory in directories)
                            {
                                string from = directory.Substring(servers.Length + 1);
                                string to = GetServer(from, HideChannelIDs, HideChannelNames);

                                string guild_json = $@"{directory}\guild.json";
                                if (File.Exists(guild_json))
                                {
                                    JObject guild = new JObject();
                                    guild["id"] = HideServerIDs ? to : from;
                                    guild["name"] = to;
                                    File.WriteAllText(guild_json, guild.ToString());
                                }
                                if(HideNicknames || HideServerIDs || HideServerNames || HideChannelIDs || HideChannelNames)
                                {
                                    string[] todels = new string[]
                                    {
                                        "bans",
                                        "channels",
                                        "emoji",
                                        "webhooks",
                                    };
                                    foreach (string todel in todels)
                                        if (File.Exists($@"{directory}\{todel}.json"))
                                            File.Delete($@"{directory}\{todel}.json");
                                }
                                if (HideChannelIDs)
                                    Directory.Move(directory, $@"{servers}\{to}");
                                res[HideServerIDs ? to : from] = to;
                            }
                            File.WriteAllText(file, res.ToString());
                        }
                    }
                }
                catch { form.Invoke(Error, "ServersFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region Applications
                max = 5;
                form.Invoke(Label, "Applications");
                try
                {
                    string applications = $@"{Ziper.TempPath}\account\applications";
                    if (Directory.Exists(applications))
                    {
                        if (DeleteApplication)
                            Directory.Delete(applications, true);
                        else if(HideApplication)
                        {
                            string[] directories = Directory.GetDirectories(applications);
                            string[] keep = new string[]
                            {
                                "hook",
                                "bot_public",
                                "bot_require_code_grant",
                                "flags",
                                "rpc_application_state",
                                "store_application_state",
                                "verification_state",
                            };
                            int i = 0;
                            foreach (string directory in directories)
                            {
                                string file = $@"{directory}\bot-avatar.png";
                                if (File.Exists(file))
                                    File.Delete(file);
                                file = $@"{directory}\application.json";
                                string name = "";
                                if (File.Exists(file))
                                {
                                    JObject res = new JObject();
                                    JObject js = JObject.Parse(File.ReadAllText(file));

                                    res["name"] = name = !HideNicknames && js.ContainsKey("name") ? (string)js["name"] : $"application_{i}";
                                    res["description"] = !HideNicknames && js.ContainsKey("description") ? js["description"] : "";
                                    res["summary"] = !HideNicknames && js.ContainsKey("summary") ? js["summary"] : "";

                                    foreach(string key in keep)
                                        if (js.ContainsKey(key))
                                            res[key] = js[key];
                                    File.WriteAllText(file, res.ToString());
                                }
                                Directory.Move(directory, $@"{applications}\{name}");
                            }
                        }
                    }
                }
                catch { form.Invoke(Error, "ApplicationsFailed"); return; }
                form.Invoke(Value, start += max);
                #endregion

                #region Zip
                max = 5;
                form.Invoke(Label, "Zip");
                string tozip;
                {
                    tozip = $"{Path[0..^4]}-{Settings.GetText("Anonymize")}.zip";
                    int i = 1;
                    do
                    {
                        if (Ziper.Zip(tozip))
                            break;
                        tozip = $"{Path[0..^4]}-{Settings.GetText("Anonymize")} ({i++}).zip";
                    } while (i < 10);
                    if (!(i < 10))
                    {
                        form.Invoke(Error, "ZipFailed");
                        return;
                    }
                    Directory.Delete(Ziper.TempPath, true);
                }
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

                form.Invoke(End, tozip);
            });
            t.Start();
        }
    }
}