chrome.tabs.onUpdated.addListener(function(tabId, changeInfo, tab) {
  if (
    tab.url.indexOf('https://rink.hockeyapp.net/manage/apps/') === -1 ||
    changeInfo.status !== 'complete'
  ) {
    return;
  }

  chrome.tabs.executeScript(
    tabId,
    {
      code:
        'var injected = window.monoSymbolicatorHelperInjected; window.monoSymbolicatorHelperInjected = true; injected;',
      runAt: 'document_end'
    },
    function(res) {
      if (
        chrome.runtime.lastError || // don't continue if error (i.e. page isn't in permission list)
        res[0] // value of `injected` above: don't inject twice
      ) {
        console.log('executeScript', chrome.runtime.lastError, res);
        return;
      }

      getStackTrace(tab.id);
    }
  );

  const getStackTrace = tabId => {
    chrome.tabs.sendMessage(tabId, { api: 'getStackTrace' }, handleStackTrace);
  };

  const handleStackTrace = response => {
    if (!response.packageName) return;

    // make request to mono-symoblicate-helper service

    var url = 'http://localhost:5000/symbolicate';
    console.log(`calling symbolicate service ${url}`);
    
    var formData = new FormData();
    formData.append('packageName', response.packageName);
    formData.append('versionCode', response.versionCode);
    formData.append('stackTrace', response.stackTrace);
    fetch(url, { method: 'POST', body: formData })
      .then(response => response.text())
      .catch(err => alert(`Error calling symbolicate service. Make sure service has started.\n\n${err}`))
      .then(result => updateStackTrace(tabId, result))
      .catch(err => alert(`Error updating stack trace\n${err}`));
    };

  const updateStackTrace = (tabId, stackTrace) => {
    chrome.tabs.sendMessage(tabId, {
      api: 'updateStackTrace',
      stackTrace: stackTrace
    });
  };
});
