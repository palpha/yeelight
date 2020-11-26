using Yeelight.Core;
using System.Threading;

namespace Yeelight.Tests.Core
{
	public static class TestMessages
	{
		private static int idSeq = 11748;

		public static Device NewSearchResponse()
		{
			var idInt = Interlocked.Increment( ref idSeq );
			var id = "0x0000000011748cfe".Replace( "11748", idInt.ToString() );
			return Device.Parse( SEARCH_RESPONSE.Replace( "0x0000000011748cfe", id ) );
		}

		public const string SEARCH_RESPONSE =
			@"HTTP/1.1 200 OK
Cache-Control: max-age=3600
Date: 
Ext: 
Location: yeelight://192.168.1.76:55443
Server: POSIX UPnP/1.0 YGLC/1
id: 0x0000000011748cfe
model: color4
fw_ver: 30
support: get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music udp_new udp_keep_alive chroma
power: off
bright: 100
color_mode: 2
ct: 3907
rgb: 6399
hue: 234
sat: 100
name: ";

		public const string ADVERTISEMENT =
			@"NOTIFY * HTTP/1.1
Host: 239.255.255.250:1982
Cache-Control: max-age=3600
Location: yeelight://192.168.1.76:55443
NTS: ssdp:alive
Server: POSIX, UPnP/1.0 YGLC/1
id: 0x0000000011748cfe
model: color4
fw_ver: 30
support: get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music udp_new udp_keep_alive chroma
power: off
bright: 100
color_mode: 2
ct: 3907
rgb: 6399
hue: 234
sat: 100
name: ";

		public const string CRON_GET_RESPONSE =
			"{\"id\":1, \"result\":[{\"type\": 0, \"delay\": 15, \"mix\": 0}]}";

		public const string CRON_ADD_RESPONSE =
			"{\"id\":2, \"result\":[\"ok\"]}";

		public const string PROP_GET_RESPONSE =
			"{\"id\":3, \"result\":[\"on\",\"\",\"100\"]}";

		public const string NOTIFICATION =
			"{\"method\":\"props\",\"params\":{\"power\":\"on\", \"bright\":10}}";

		public const string BIG_NOTIFICATION =
			"{\"method\":\"props\",\"params\":{\"smart_switch\":0,\"init_power_opt\":1,\"hue\":234,"
			+ "\"sat\":100,\"music_on\":0,\"name\":\"\",\"lan_ctrl\":1,\"delayoff\":0,"
			+ "\"flow_params\":\"0,1,3000,1,16711680,100,3000,1,65280,100,3000,1,255,100,3000,1,"
			+ "9055202,100\",\"flowing\":0,\"color_mode\":2,\"save_state\":1,\"ct\":3907,"
			+ "\"bright\":100,\"power\":\"on\"}}";

		public const string ERROR_RESPONSE =
			"{\"id\":0, \"error\":{\"code\":-1, \"message\":\"invalid command\"}}";
	}
}
