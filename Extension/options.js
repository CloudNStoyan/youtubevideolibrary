// Copyright 2018 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

'use strict';

let page = document.getElementsByClassName('leftcolumn')[0];

GetHtmlFromServer();

function GetHtmlFromServer() {
fetch("http://localhost:2905/videos").then(function(response) { 
	response.text().then(function(text) {
			page.innerHTML = text;
			console.log(text);
	});
})	
};



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