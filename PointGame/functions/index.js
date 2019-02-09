// The Cloud Functions for Firebase SDK to create Cloud Functions and setup triggers.
var functions = require('firebase-functions');

// The Firebase Admin SDK to access the Firebase Realtime Database.
const admin = require('firebase-admin');
admin.initializeApp(functions.config().firebase);





//document.write("<script type='text/javascript' src="https://www.gstatic.com/firebasejs/5.7.2/firebase.js"><"+"/script>");
//require('https://www.gstatic.com/firebasejs/5.7.2/firebase.js');

/*

   src="https://www.gstatic.com/firebasejs/5.5.2/firebase-app.js"
   src="https://www.gstatic.com/firebasejs/5.5.2/firebase-auth.js"
   src="https://www.gstatic.com/firebasejs/5.5.2/firebase-database.js"
   src="https://www.gstatic.com/firebasejs/5.5.2/firebase-messaging.js">
*/
var firebase = require("firebase");
  var config = {
    apiKey: "AIzaSyA_imM5VWH04xiG7Z4A5e6pI5vzTIbbSqw",
    authDomain: "treasureone-4472e.firebaseapp.com",
    databaseURL: "https://treasureone-4472e.firebaseio.com/",
    projectId: "treasureone-4472e",
    storageBucket: "treasureone-4472e.appspot.com",
    messagingSenderId: "641308195675",
  };

  firebase.initializeApp(config);

var defaultDatabase = firebase.database();


var LottoSeriesRef = 1000000;
var LottoRefNumber;
var Lottocount;
var arrLottoUser = {}//new Array();
var arrLottoUserKey = new Array();
var LottoCurSeries;
var LottoTodaySeries;


var AutoLottoSelectMode = 0;
//SetLottoCount();

 function SetLottoCount()
 {
   firebase.database().ref('/AutoLottoSelect').once('value', function(snapshot) {
     var message = snapshot.val();
     AutoLottoSelectMode = message;

    // console.log('AutoLottoSelectMode  '  + AutoLottoSelectMode);

       if(AutoLottoSelectMode)
       {
         showClock();
       }
       else
       {
         clearTimeout(timer);
       }
 });
 }


  function SetLottoUserData()
  {

    firebase.database().ref('/LottoCount').once('value', function(snapshot) {
      var message = snapshot.val();
      Lottocount = message;
      $("#LOTTO_GROUP").text(Lottocount);

      firebase.database().ref('/LottoCurSeries').once('value', function(snapshot) {
        var message = snapshot.val();
        LottoCurSeries = message;
        LottoCurSeries += LottoSeriesRef;
      //  LottoCurSeries = 7;
        //console.log('/Lotto/'+LottoCurSeries+'_L');

        var tempIdx = 0;
          var dbTestRef = firebase.database().ref('/Lotto/'+LottoCurSeries+'_L')
          dbTestRef.on('child_added', function(data){

            //console.log(data.val(), 'key: ', data.key)
            arrLottoUser[data.key] = data.val();
            arrLottoUserKey[tempIdx] = data.key;
            tempIdx++;
          //  console.log(arrLottoUserKey);
          //  console.log(Lottocount + '___' + arrLottoUserKey.length);
            if( Lottocount === arrLottoUserKey.length +1)
            {
              var result = Math.floor(Math.random() * arrLottoUserKey.length);
          //    console.log(result);
              result = arrLottoUserKey[result];

            //  console.log(result);

              firebase.database().ref('/LottoLuckyNumber/'+LottoCurSeries+'_L').set(arrLottoUser[result])
              firebase.database().ref('/LottoLuckyGroup/'+LottoCurSeries+'_L').set(result)

              var LottoRandNumber = Math.floor(Math.random() * 20) + 1;
              firebase.database().ref('/LottoRefNumber').set(LottoRandNumber)

              firebase.database().ref('/Lotto/'+LottoCurSeries+'_L').remove();
              LottoCurSeries = LottoCurSeries - LottoSeriesRef;
              LottoCurSeries += 1;
              firebase.database().ref('/LottoCurSeries').set(LottoCurSeries)
              firebase.database().ref('/LottoCount').set(1)
            }


          })


        //console.log(LottoCurSeries);
      });
  });

  }



var timer;

//var arrLottoSelectTime = ['09:00:00', '12:00:00', '15:00:00', '18:00:00'];
var arrLottoSelectTime = ['00:00:00', '03:00:00', '06:00:00', '09:00:00'];

Number.prototype.to2 = function(){return this<10?'0'+this:this;};
 Date.prototype.getHMS = function(){
       return this.getHours().to2() + ':' + this.getMinutes().to2() + ':'
            + this.getSeconds().to2();
 }


function showClock(){
        //console.log((new Date()).getHMS());

        for(var i=0; i<4; i++) {              //console.log(arrLottoSelectTime[i]);
         if((new Date()).getHMS() === arrLottoSelectTime[i])
         {
              console.log(arrLottoSelectTime[i]);
              SetLottoUserData();
         }
      }

        timer = setTimeout(showClock,1000);
    }

   //showClock();

    exports.dbWrite = functions.database.ref('/AutoLottoSelect').onWrite((change, context) => {
     const beforeData = change.before.val(); // data before the write
     const afterData = change.after.val(); // data after the write
    AutoLottoSelectMode = afterData;

    console.log("AutoLottoSelectMode" + AutoLottoSelectMode);
    if(AutoLottoSelectMode)
    {
      showClock();
    }
    else
    {
      clearTimeout(timer);
      //console.log('0');
    }

    return AutoLottoSelectMode;

  });
