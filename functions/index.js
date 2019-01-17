// The Cloud Functions for Firebase SDK to create Cloud Functions and setup triggers.
const functions = require('firebase-functions');

// The Firebase Admin SDK to access the Firebase Realtime Database.
const admin = require('firebase-admin');
admin.initializeApp();


// Take the text parameter passed to this HTTP endpoint and insert it into the
// Realtime Database under the path /messages/:pushId/original
exports.addMessage = functions.https.onRequest((req, res) => {
  // Grab the text parameter.
  const original = req.query.text;
  // Push the new message into the Realtime Database using the Firebase Admin SDK.
  return admin.database().ref('/messages').push({original: original}).then((snapshot) => {
    // Redirect with 303 SEE OTHER to the URL of the pushed object in the Firebase console.
    return res.redirect(303, snapshot.ref.toString());
  });
});


/*
Number.prototype.to2 = function(){return this<10?'0'+this:this;};
 Date.prototype.getHMS = function(){
       return this.getHours().to2() + ':' + this.getMinutes().to2() + ':'
            + this.getSeconds().to2();
 }
 function GetLotto(){
         console.log('지금 시각 : ' + (new Date()).getHMS() );
 }
 function GetTime(){
 //        var 자정='00:00:00';
       var time ='15:17:55';
      // if((new Date()).getHMS() == time){
             GetLotto();
    //  }
      window.setTimeout(GetTime,1000); // 1초마다 반복
 }
 GetTime();
*/
