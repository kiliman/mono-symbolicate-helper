/* Listen for messages */
console.log('content.js loaded');
chrome.runtime.onMessage.addListener(function(msg, sender, sendResponse) {
  
  const getStackTrace = () => {
    const packageName = getPackageName();
    if (!packageName) {
      alert('Go to application overview to capture packageName');
      return {};
    }
    const version = document.querySelector('.nav-version .nav-title');
    if (!version) {
      return {};
    }
      
    const versionCode = version.innerText.match(/\((\d+)\)/)[1];

    const node = document.querySelector(
      'div.stacktrace-wrapper div.raw-wrapper pre'
    );
    if (!node) {
      return {};
    }

    const stackTrace = node.innerText;

    return { packageName, versionCode, stackTrace };
  };

  const getPackageName = () => {
    // get app id from url
    const appId = document.location.href.match(/\/apps\/(\d+)/)[1];
    let packageName = localStorage.getItem(`msh-appId-${appId}`);
    if (!packageName) {
      var url = `https://rink.hockeyapp.net/manage/apps/${appId}`;
      if (document.location.href === url) {
        packageName = document.querySelector('a[data-name="bundle_identifier"]').innerText;
        localStorage.setItem(`msh-appId-${appId}`, packageName);
      }
    }
    return packageName;
  };

  const updateStackTrace = stackTrace => {
    console.log(stackTrace);

    // update raw stack trace
    document.querySelector(
      'div.stacktrace-wrapper div.raw-wrapper pre'
    ).innerText = stackTrace;

    // handle formatted stack trace
    let lines = stackTrace.split('\n');
    //const re = /at (?<namespace>.+?)\.(?<class>[^.]+)\.(?<method>[^.]+)\s*\((?<filename>[^:)]+):?(?<line>\d*)\)?(.*?in\s*)?(?<filename2>.*?:(?<line2>\d+))/;
    const re = /at (.+?)\.([^.]+)\.([^.]+)\s*\(([^:)]+):?(\d*)\)?(.*?in\s*)?(.*?:(\d+))/;
    let html = '';
    lines.forEach(l => {
      const matches = l.match(re) || [];
      let [, namespace, klass, method, filename, , , filename2, line] = matches;
      if (namespace) {
        html += `<div class="frame line">`;
        html += `<span class="namespace">${namespace}.</span>`;
        html += `<span class="class" title="${namespace}.${klass}">${klass}</span>`;
        html += `<span class="method"><span class="dot">.</span>`;
        html += `<span class="name">${method}</span><span class="paren">()</span></span>`;

        if (filename2[0] === ':') {
          html += `<span class="location"><span class="file">${filename}</span>:${line}</span>`;
        } else {
          html += `<span class="location"><span class="file">${filename2}</span></span>`;
        }
        html += `</div>`;
      } else {
        html += `<div class="unrecognized line">${l}</div>`;
      }
    });

    const div = document.querySelector(
      'div.stacktrace-wrapper div.rich-wrapper div.stacktrace'
    );
    const append = localStorage.getItem('msh-append') === 'true';
    if (append) {
      div.innerHTML += '<hr/>' + html;
    } else {
      div.innerHTML = html;
    }

  };

  if (msg.api === 'getPackageName') {
    sendResponse(getPackageName());
  } else if (msg.api === 'getStackTrace') {
    sendResponse(getStackTrace(msg.packageName));
  } else if (msg.api === 'updateStackTrace') {
    updateStackTrace(msg.stackTrace);
  }
});
