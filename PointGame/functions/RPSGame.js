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

var timer;
var arrRPSGameTime = ['00:00:00', '03:00:00', '06:00:00', '09:00:00'];

Number.prototype.to2 = function(){return this<10?'0'+this:this;};
 Date.prototype.getHMS = function(){
       return this.getHours().to2() + ':' + this.getMinutes().to2() + ':'
            + this.getSeconds().to2();
 }


function showClock(){
        //console.log((new Date()).getHMS());
        for(var i=0; i<4; i++) {              //console.log(arrLottoSelectTime[i]);
         if((new Date()).getHMS() === arrRPSGameTime[i])
         {
              console.log(arrRPSGameTime[i]);
         }
      }

        timer = setTimeout(showClock,1000);
    }


    exports.dbRPSStatusWrite = functions.database.ref('/RPSUserCount').onWrite((change, context) => {
     const beforeData = change.before.val(); // data before the write
     const afterData = change.after.val(); // data after the write


     console.log("RPSUserCount" + afterData);

  });


  exports.dbRPSStatusWrite = functions.database.ref('/RPSUserCount').onWrite((change, context) => {
   const beforeData = change.before.val(); // data before the write
   const afterData = change.after.val(); // data after the write
   
   console.log("RPSUserCount" + afterData);

});

  exports.dbRPSStatusWrite = functions.database.ref('/RPSGameStatus').onWrite((change, context) => {
   const beforeData = change.before.val(); // data before the write
   const afterData = change.after.val(); // data after the write

  console.log("RPSGameStatus" + afterData);

  var afterSeries = afterData * 1;
  if(afterSeries < 0)
  {
    afterSeries += LottoRefNumber;
    console.log("LottoCurSeries Fixed " + afterSeries);
    firebase.database().ref('/LottoCurSeries').set(afterSeries);
  }
  return afterSeries;

});

//SetLottoCount();
