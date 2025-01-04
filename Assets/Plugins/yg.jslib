mergeInto(LibraryManager.library, {
  SDKInit: function () {
    if (typeof ysdk === 'undefined') {
        return false;
    }
    else {
        return true;
    }
  },

  GameReady: function () {
    ysdk.features.LoadingAPI.ready();
  },

  ShowAdv: function () {
    ysdk.adv.showFullscreenAdv({
        callbacks: {
            onClose: function(wasShown) {
                console.log('Ad shown successfully');
            },
            onError: function(error) {
                console.log('Ad error!');
            }
        }
    })
  },

  ShowRewarded: function() {
    ysdk.adv.showRewardedVideo({
        callbacks: {
            onRewarded: () => {
                console.log('Rewarded!');
                myGameInstance.SendMessage('_yandexGames', 'RewardedSuccess');
            },
            onError: (e) => {
                console.log('Error while open rewarded ad:', e);
            }
        }
    })
  },

  GetLang: function () {
    var lang = ysdk.environment.i18n.lang;
    var bufferSize = lengthBytesUTF8(lang) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(lang, buffer, bufferSize);
    return buffer;
  },
});