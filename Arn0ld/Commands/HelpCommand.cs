using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Arn0ld.Commands
{
    public class HelpCommand : ModuleBase<CommandContext>
    {
        [Command("help")]
        public async Task Help()
        {
            string helpmessage = "!lock - zablokuj kanal publiczny, w ktorym aktualnie przebywasz\n!invite - zapros innego użytkownika do publicznego kanalu, ktory wczesniej zablkowales\n";

            var dm = await Context.User.CreateDMChannelAsync();

            await dm.SendMessageAsync(helpmessage);
        }
    }
}
