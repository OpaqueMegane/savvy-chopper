/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
"use strict";

// Start the main app logic.
requirejs(
  [ 'hft/gameclient',
    'hft/commonui',
    'hft/misc/misc',
    'hft/misc/mobilehacks',
    'hft/misc/cookies',
  ], function(
    GameClient,
    CommonUI,
    Misc,
    MobileHacks,
    Cookie) {

  var g_name = "";
  var g_client;
  var playerNumberDisplayElement;
  var ignoreCookie = false;
  var haveId = false;

  var globals = {
  };
  Misc.applyUrlSettings(globals);
  MobileHacks.fixHeightHack();

  /*function $(id) {
    return document.getElementById(id);
  }*/

  function onScored(data) {
    // do something great

  };

  function handleSetID(object) {
		haveId = true;
		
		var playerNumber = object.id.split('|')[1];
		
      playerNumberDisplayElement.innerHTML = "you-are_PLAYER " + playerNumber;//object.id;

      console.log("my id: " + object.id);

      cookieID.set(object.id);

  };

  var mySound = new buzz.sound("heli", {

      formats: ["wav","mp3"]

  });

  mySound.play()
      .loop()
      .setVolume(100);
  mySound.setSpeed(Math.random() * 2 + 0.5);

      //mySound.setVolume() // 0-100

  var flying = false;
  var currentTime = 0;
  var lastTime = 0;
  var intervalTime = 100;

      ///////
  /*var node = new THREE.Object3D();

  var orientation = new THREE.Euler();

  var controls = new THREE.DeviceOrientationControls(node, true);

  controls.connect();

  controls.update();*/



      ////////

  window.onkeypress = function () {
      // ramp volume for some time then fade out if keys aren't pressed


      if (!flying) {

          console.log("fading in");
          mySound.fadeIn(500);

          var check = setInterval(function () {
          
          
              // check time difference
              currentTime += intervalTime;
              var timeDifference = currentTime - lastTime;

              console.log("checking interval, timeDifference: " + timeDifference);

              if (timeDifference > 2000) {

                  currentTime = 0;
                  clearInterval(check);
                  mySound.fadeOut(1000, function () {

                      flying = false;

                  });

              }

          }, intervalTime);

      } else {

          lastTime = currentTime;

      }
      flying = true;

  }


  function sendPartnerRequest(idOfPartner)
  {

      g_client.sendCmd('requestPartner', {

          partnerId: idOfPartner

      });

  }

  function setCharacter(data) {
		partnerComboBox.style.display = "none";
		$("#fruit_icon").attr("src", "fruit_icons/" + data.character + ".png");
		
		if (data.horizontalController)
		{
			$("#instructions").attr("src", "instruction_horiz.gif");
		}
		else
		{
		$("#instructions").attr("src", "instruction_vert.gif");
		}
		
		//$("#instructions").attr("src", "fruit_icons/" + data.character + ".png");
		
  }

      //set up character number display
      ////////////////////
         playerNumberDisplayElement = document.createElement("p");

         var nodee = document.createTextNode("Waiting for player number...");

         playerNumberDisplayElement.style.marginTop = "50px";
         playerNumberDisplayElement.style.marginLeft = "15px";
         playerNumberDisplayElement.style.fontSize = "2.5em";
		 //playerNumberDisplayElement.style.fontFamily = '"Comic Sans MS", "Comic Sans", cursive';
		 playerNumberDisplayElement.style.fontWeight = "1200";
		 playerNumberDisplayElement.style.color = "black";
		 playerNumberDisplayElement.style.textShadow = "5px 5px #ffffff";
		 playerNumberDisplayElement.style.zIndex = "999999";

         playerNumberDisplayElement.appendChild(nodee);

            var element = document.getElementById("characterSkin");

            element.appendChild(playerNumberDisplayElement);

      ///////////




      ///////////
            var partnerComboBox = document.createElement("select");
            partnerComboBox.style.marginTop = "50px";
			partnerComboBox.style.marginLeft = "15px";
            partnerComboBox.style.position = "absolute";
			
			         partnerComboBox.style.fontSize = "1.5em";
		 //partnerComboBox.style.fontFamily = '"Comic Sans MS", "Comic Sans", cursive';
		 partnerComboBox.style.fontWeight = "1200";
			
            partnerComboBox.style.zIndex = "99999";
            element.appendChild(partnerComboBox);

            var opt = document.createElement("option");


			opt.value = "choose your partner!";
            opt.textContent = "choose your partner";
            partnerComboBox.appendChild(opt);

			opt = document.createElement("option");
            opt.value = "don't care";

            opt.textContent = "don't care";

            partnerComboBox.appendChild(opt);

            for (var i = 0; i < 24; i++)

            {

               var opt2 = document.createElement("option");

                  opt2.value = i;

                  opt2.textContent = "player " + i;

                  partnerComboBox.appendChild(opt2);

            }



            var onSelectPartner = function (eventData) {
            	var request =  partnerComboBox.selectedIndex - 2;
            	if (request < 0)
            	{
            		request = -1;
            	}
                console.log("onSelectPartner");
                g_client.sendCmd('requestPartner', {
                    //should check if you're requesting yourself [!!!]
                    partnerId:request,
                });
            };



            partnerComboBox.onchange = onSelectPartner;

      /////////////////////
      /*
      var iconDiv = document.createElement("div");
      element.appendChild(iconDiv);
      */

      

      /////////////////////

      //set a default background
  var characterElement = document.getElementById('characterSkin'); // fullscreen background div
  characterElement.style.backgroundImage = "url('lemonCharacter.gif')";
      //setCharacter("apple");
  

  g_client = new GameClient();

  g_client.addEventListener('scored', onScored);
  g_client.addEventListener('character', setCharacter);
  g_client.addEventListener('assignID', handleSetID);



  CommonUI.setupStandardControllerUI(g_client, globals);

  var color = Misc.randCSSColor();
  //g_client.sendCmd('setColor', { color: color });
  document.body.style.backgroundColor = color;

  var cookieID = new Cookie("playerNumber"); //composed of a session ID and a unique player ID


function check4Id()
{
if (!haveId)
{
 if (cookieID.get() == undefined || ignoreCookie) {

      console.log("cookie was undefined, set one");
      g_client.sendCmd('needID');

  }
  else {

      console.log("cookie was defined, send message to unity");
      g_client.sendCmd('checkID', { id: cookieID.get() });

  }
}
}
 
  
  setInterval(check4Id, 3000);



  var sendDeviceOrientation = function(eventData) {
    /*g_client.sendCmd('orient', {
      gamma: eventData.gamma,
      beta: eventData.beta,
      alpha: eventData.alpha,
    });*/
  };

  var sendDeviceOrientation2 = function (eventData) {

      controls.setOrientation(eventData);

      controls.update();

      orientation.setFromQuaternion(node.quaternion, 'YXZ');

      g_client.sendCmd('orient', {

          gamma: orientation.z,

          beta: orientation.y,

          alpha: orientation.x,

      });

  };

  if (!window.DeviceOrientationEvent) {
    alert("Your device/browser does not support device orientation. Sorry");
    return;
  }

      ///////////////
  var quantize = function (v) {

      return v;//Math.floor(v);

  };
  var sendDeviceAcceleration = function (eventData) {

      var accel = eventData.acceleration || eventData.accelerationIncludingGravity;

      //var rot = eventData.rotationRate || { alpha: 0, gamma: 0, beta: 0 };

      var interval = eventData.interval || 1;

      var msg = {

          alpha: quantize(10*accel.x / interval),

          beta: quantize(10 * accel.y / interval),

          gamma: quantize(10 * accel.z / interval),

      };



      g_client.sendCmd('orient', msg);

  };

  window.addEventListener('devicemotion', sendDeviceAcceleration, false);
  
      ////////////////

  window.addEventListener('deviceorientation', sendDeviceOrientation, false);
  window.onkeydown = function (eventData) {
      
      var send = -999;
      console.log(String.fromCharCode(eventData.keyCode))
      if (String.fromCharCode(eventData.keyCode) == 'Z')
      {
          send = 999;
      }
      else if (String.fromCharCode(eventData.keyCode) == 'P')
      {
      	send = 777;
      }

      g_client.sendCmd('orient', {
          gamma: 0,
          beta: 0,
          alpha: send,
      });
  };
});


