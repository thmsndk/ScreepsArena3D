
Notes on arena screeps with fiddler
POST /api/auth/login HTTP/1.1
{"ticket":"14000000aa44c52326f379bac733020001001001ab199460180000000100000002000000031f9d067c3dd995f50b5b0308000000b20000003200000004000000c733020001001001a85a1100357cba052101a8c000000000d1da8b60518aa760010019e7050000000000535fd501dc6202479daa20b9384e0c3613e669bfd9f64a6a5e7808cc15c9608bcdfaca19c3ab2100e3d6107cf140ddd45f90ae5613d2731af110f57d63cd25dc3d5a35043c9a7b57c79fed74101f48b88b79d6fb52e83f6aa2cd3dd9c201b04a84c0dae2a9388c83947632d3bccd0b81794ad93d6d4eb74828cf5f9da512a221

response
the response sets a cookie
Set-Cookie: connect.sid=s%3AIWFxQl9joKpPDQW0n7BHShAIJHGb5gp2.%2F%2BxitjKlZN8bMjvsd8bJtgKdrI4YQrz%2FYdiqoal5xkc; Path=/; HttpOnly

this cookie seems to be sent to authenticate

{"ok":1,"_id":"609419addf54262f11f5d844","steam":{"steamid":"76561197960410055","personaname":"thmsn","profileurl":"https://steamcommunity.com/id/thmsn/"},"registeredDate":"2021-05-06T16:30:37.168Z","username":"thmsn"}

This seems like it is being used for authentication
https://github.com/greenheartgames/greenworks/blob/master/docs/authentication.md

Perhaps we can figure out how to obtain a ticket from here
https://partner.steamgames.com/doc/features/auth

Unity:
http://steamworks.github.io/installation/
http://steamworks.github.io/gettingstarted/
https://docs.microsoft.com/en-us/gaming/playfab/features/authentication/platform-specific-authentication/steam-unity



GET https://arena.screeps.com/api/arena/list HTTP/1.1
HTTP/1.1 200 OK
Server: nginx/1.14.2
Date: Thu, 06 May 2021 21:02:42 GMT
Content-Type: application/json; charset=utf-8
Content-Length: 266
Connection: keep-alive
X-Powered-By: Express
ETag: W/"10a-vC50l/xFQj5GJ1x2YEYVMd3LADM"
Set-Cookie: connect.sid=s%3ApFkvxen6tuYVLV7kBBGIptTm34QaaV2I.yZ34EoYPpdwKVeQowui1dQBYTFfIfSJKrzquUBp9oXw; Path=/; HttpOnly

```json
{
	"ok": 1,
	"arenas": [{
			"_id": "606873c364da921cb49855f7",
			"name": "Capture the Flag",
			"advanced": false,
			"rating": 481,
			"rank": 12,
			"games": 36,
			"qualifying": false
		}, {
			"_id": "608056514cb5ae2f245d57a0",
			"name": "Capture the Flag",
			"advanced": true,
			"rating": 0,
			"games": 2,
			"qualifying": true
		}
	]
}
```


you push your code when you start a game
POST https://arena.screeps.com/api/game/start HTTP/1.1
Host: arena.screeps.com

------WebKitFormBoundaryjti6LWBWZTf63iiB
Content-Disposition: form-data; name="arena"

606873c364da921cb49855f7
------WebKitFormBoundaryjti6LWBWZTf63iiB
Content-Disposition: form-data; name="code"; filename="blob"
Content-Type: application/zip


----- then we recieve this a bunch of times
 it seems like a request is made untill status = finished
GET https://arena.screeps.com/api/game/60945968444a8b744b359dc7 HTTP/1.1


```json
{
	"ok": 1,
	"game": {
		"_id": "60945968444a8b744b359dc7",
		"ratingHistory": {
			"previousRating": 279,
			"rating": 279,
			"previousRank": 17,
			"rank": 17,
			"calibrating": false
		},
		"codes": [{
				"_id": "6094999c444a8b418835a10d",
				"user": "609498b8df54262f11aa89ed",
				"version": 2
			}, {
				"_id": "60964be9444a8b645935aa58",
				"user": "609419addf54262f11f5d844",
				"version": 24
			}
		],
		"users": [{
				"_id": "609419addf54262f11f5d844",
				"username": "thmsn"
			}, {
				"_id": "609498b8df54262f11aa89ed",
				"username": "Golden Eagle"
			}
		],
		"user": "609419addf54262f11f5d844",
		"arena": "606873c364da921cb49855f7",
		"game": {
			"_id": "60945968444a8b744b359dc7",
			"status": "running",
			"createdAt": "2021-05-06T21:02:32.383Z",
			"terrain": "1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111122222220.....",
			"playerColor": ["#FF3333", "#5555FF"],
			"usersCode": ["6094516d444a8b76f2359d1a", "60944cc3444a8ba07d359ccf"],
			"result": {
				"status": "ok",
				"winner": 0.5

			}
		},
		"meta": {
			"ticks": 600
		}
	}
}
```



GET https://arena.screeps.com/api/arena/606873c364da921cb49855f7/rating-history?offset=0&limit=10 HTTP/1.1
Host: arena.screeps.com
Connection: keep-alive
Accept: application/json, text/plain, */*
User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) arena/0.0.12 Chrome/89.0.4389.69 Electron/12.0.0 Safari/537.36
Sec-Fetch-Site: cross-site
Sec-Fetch-Mode: cors
Sec-Fetch-Dest: empty
Accept-Encoding: gzip, deflate, br
Accept-Language: en-US
Cookie: connect.sid=s%3ApFkvxen6tuYVLV7kBBGIptTm34QaaV2I.yZ34EoYPpdwKVeQowui1dQBYTFfIfSJKrzquUBp9oXw
If-None-Match: W/"1aff-P58ILbsjfAlTf1BeJuu5vfVS3o8"

HTTP/1.1 200 OK
Server: nginx/1.14.2
Date: Thu, 06 May 2021 21:02:42 GMT
Content-Type: application/json; charset=utf-8
Content-Length: 6857
Connection: keep-alive
X-Powered-By: Express
ETag: W/"1ac9-Te3Z+pZRPtM/jPwQwNgNMfIP+ZE"
Set-Cookie: connect.sid=s%3ApFkvxen6tuYVLV7kBBGIptTm34QaaV2I.yZ34EoYPpdwKVeQowui1dQBYTFfIfSJKrzquUBp9oXw; Path=/; HttpOnly

response
```json
{
	"ok": 1,
	"history": [{
			"_id": "60945968c32b67507b123f3b",
			"game": {
				"_id": "60945968444a8b744b359dc7",
				"status": "finished",
				"createdAt": "2021-05-06T21:02:32.383Z",
				"result": {
					"status": "ok",
					"winner": 0.5
				},
				"usersCode": ["6094516d444a8b76f2359d1a", "60944cc3444a8ba07d359ccf"]
			},
			"codes": [{
					"_id": "60944cc3444a8ba07d359ccf",
					"user": "609419addf54262f11f5d844",
					"version": 16
				}, {
					"_id": "6094516d444a8b76f2359d1a",
					"user": "609419addf54262f11f5d844",
					"version": 17
				}
			],
			"users": [{
					"_id": "609419addf54262f11f5d844",
					"username": "thmsn"
				}
			],
			"user": "609419addf54262f11f5d844",
			"arena": "606873c364da921cb49855f7",
			"ratingHistory": {
				"previousRating": 481,
				"rating": 481,
				"previousRank": 12,
				"rank": 12,
				"calibrating": false
			}
		}
	],
	"meta": {
		"length": 36
	}
}
```

pressing watch replay button seems a lot like replays from official servers
`GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/replay/0 HTTP/1.1`
we get a gziped response, and the next request is `replay/100` containing 100 objects, with gamestates
```json
[{
		"gameTime": 0,
		"objects": [{
				"type": "creep",
				"prototypeName": "Creep",
				"x": 1,
				"y": 8,
				"hits": 800,
				"hitsMax": 800,
				"body": [{
						"type": "move",
						"hits": 100
					}, {
						"type": "heal",
						"hits": 100
					}, {
						"type": "move",
						"hits": 100
					}, {
						"type": "heal",
						"hits": 100
					}, {
						"type": "move",
						"hits": 100
					}, {
						"type": "heal",
						"hits": 100
					}, {
						"type": "move",
						"hits": 100
					}, {
						"type": "heal",
						"hits": 100
					}
				],
				"spawning": false,
				"ageTime": 0,
				"fatigue": 0,
				"storeCapacity": 0,
				"store": {},
				"user": "player1",
				"actionLog": {},
				"_id": 1
			}, {
				"type": "flag",
				"prototypeName": "Flag",
				"x": 2,
				"y": 2,
				"user": "player1",
				"_id": 2
			}
		],
		"users": {
			"player1": {
				"_id": "player1",
				"username": "player1",
				"color": "#FF3333"
			},
			"player2": {
				"_id": "player2",
				"username": "player2",
				"color": "#5555FF"
			}
		}
	}
]
```
when the 100 request is made, another request is made for the console log, gzipped as well, but a json resposne is received
`GET https://arena.screeps.com/api/game/60969f8c444a8bf84135abe3/log/100 HTTP/1.1`
response:
```json
{
	"1": "....",
	...
	"9": "",
	"10": "I have 14 creeps",
	..
	"20": "I have 14 creeps",
	...
	"100": "I have 7 creeps"
}
```
