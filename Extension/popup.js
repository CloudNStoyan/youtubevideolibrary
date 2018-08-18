// Copyright 2018 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

'use strict';

let saveBtn = document.getElementById('save-btn');
let otherBtn = document.getElementById('another-btn');

saveBtn.addEventListener('click',function() {
	let youtubeVideoUrl = "";
	chrome.tabs.query({active: true, currentWindow: true}, function(tabs) {
		let activeTab = tabs[0];
		SendRequest('http:localhost:2905/',activeTab.url);
	});	
});

otherBtn.addEventListener('click',function() {
	
	chrome.tabs.executeScript(null, {
		code: "console.log(document.getElementById('eow-title').innerText);"
	});
});
function SendRequest(url,body) {
	fetch(url, {
                method: 'POST',
                headers : {
				'Content-Type':'application/x-www-form-urlencoded;charset=UTF-8'
				},
                body: body
            }).then((res) => res)
            .then((data) =>  console.log(data))
            .catch((err)=>console.log(err))
}

