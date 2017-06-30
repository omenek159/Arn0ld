using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Arn0ld
{
    class BotCore
    {
        DiscordSocketClient _client;
        CommandService _commands = new CommandService();
        private IServiceProvider _services = new ServiceCollection().BuildServiceProvider();

        SocketGuild _guild;

        string logchannel = "bot-log";
        static SocketTextChannel _logchannel;

        string token = "MzI3MzcyMzk2OTkyNTI4Mzg0.DDGdfA.uZ6gxpSi-76Sk6-0Ck9CFw7j7Yw";

        List<SocketGuildChannel> publicchannels = new List<SocketGuildChannel>();
        List<SocketGuildChannel> freepublicchannels = new List<SocketGuildChannel>();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                //DefaultRetryMode = RetryMode.AlwaysFail
            });

            await InitCommands();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Ready += Ready;
            _client.Log += Log;
            _client.UserJoined += UserJoined;
            _client.UserVoiceStateUpdated += UserVoiceStateUpdated;
            _client.ChannelCreated += ChannelCreated;
            _client.ChannelDestroyed += ChannelDestroyed;
            _client.ChannelUpdated += ChannelUpdated;

            await Task.Delay(-1);
        }

        private Task ChannelUpdated(SocketChannel channelbefore, SocketChannel channelafter)
        {
            GetPublicChannels();

            return Task.CompletedTask;
        }

        private Task ChannelDestroyed(SocketChannel channel)
        {
            GetPublicChannels();

            return Task.CompletedTask;
        }

        private Task ChannelCreated(SocketChannel channel)
        {
            GetPublicChannels();

            return Task.CompletedTask;
        }

        private async Task Ready()
        {
            _guild = _client.Guilds.FirstOrDefault();

            _logchannel = _guild.GetTextChannel(_guild.TextChannels.Where(x => x.Name == logchannel).First().Id);

            Console.WriteLine(_logchannel.Name);

            await ChannelLog("Ready!");

            await GetPublicChannels();

            //return Task.CompletedTask;
        }

        private async Task UserVoiceStateUpdated(SocketUser arg1, SocketVoiceState arg2, SocketVoiceState arg3)
        {
            await UpdatePublicChannels();
        }

        private Task UserJoined(SocketGuildUser arg)
        {
            var user = arg.Username;
            var channel = arg.VoiceChannel;

            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage arg)
        {
            //var message = arg as SocketUserMessage;

            return Task.CompletedTask;
        }

        public static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.ToString());

            return Task.CompletedTask;
        }

        public static async Task ChannelLog(string message)
        {
            var eb = new EmbedBuilder();
            eb.WithDescription(message);

            await _logchannel.SendMessageAsync("test", false, eb);
            //_logchannel.SendMessageAsync("test");

            //return Task.CompletedTask;
        }

        private async Task InitCommands()
        {
            _client.MessageReceived += HandleCommand;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if ((message.HasCharPrefix('!', ref argPos)) & !(messageParam.Author.IsBot))
            {
                var context = new CommandContext(_client, message);
                var result = await _commands.ExecuteAsync(context, argPos);
                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private async Task UpdatePublicChannels()
        {
            await GetPublicChannels();

            if (freepublicchannels.Count < 2)
            {
                var message = await _guild.CreateVoiceChannelAsync("Public " + (publicchannels.Capacity + 1));
            }

            if (freepublicchannels.Count > 2 & publicchannels.Count > 6)
            {
                await _guild.GetChannel(publicchannels.Last().Id).DeleteAsync();
            }
        }

        private Task GetPublicChannels()
        {
            publicchannels.Clear();
            freepublicchannels.Clear();

            foreach (SocketGuildChannel channel in _guild.VoiceChannels)
            {
                if (channel.Name.Contains("Public "))
                {
                    publicchannels.Add(channel);

                    if (channel.Users.Count < 1)
                    {
                        freepublicchannels.Add(channel);
                    }
                }
            }

            publicchannels.OrderBy(x => x.Name).ToList();
            freepublicchannels.OrderBy(x => x.Name).ToList();

            return Task.CompletedTask;
        }        
    }
}
