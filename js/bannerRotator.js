/* This script and many more are available free online at
The JavaScript Source!! http://javascript.internet.com
Created by: Scragar | Licensed under: Public Domain
 */

function BannerRotator(){
// first, defaults:
  this.timer = 2;
  this.bannerNum = 0;// -1 = random
// then normal init work
  this.banners = [];
  this.binding = null;
  this.timeout = null;

  this.add = function(){// add a banner to the array
    this.banners[this.banners.length] = [arguments[0], arguments[1]];
  }

  this.bind = function(){// bind to an element
    if(typeof arguments[0] == 'string')
      this.binding = document.getElementById(arguments[0]);
    else
      this.binding = arguments[0];
    this.rotate();
  }

  this.rotate = function(){// the actual image rotator
    if( ! this.empty())
      return;
    var showNum, tmpA = document.createElement('a'), tmpImg = document.createElement('img');

    if(this.bannerNum < 0)// random
      showNum = Math.floor(Math.random()*this.banners.length);
    else// syncronous
      showNum = this.bannerNum=(++this.bannerNum >= this.banners.length)?0:this.bannerNum;

    tmpA.href = this.banners[showNum][0];
    tmpImg.src = this.banners[showNum][1];
    tmpA.appendChild(tmpImg);
    this.binding.appendChild(tmpA);
  }

  this.empty = function(){// empty the element if possible
    if(this.binding == null)
      return false;
    while(this.binding.hasChildNodes())
      this.binding.removeChild(this.binding.firstChild);
    return true;
  }

  this.startTimer = function(){// start the loop, normaly done during page load.
    this.stopTimer();
    this.timeout = window.setInterval(
      (function(x){
        return function(){
          x.rotate();
        }
      })(this), this.timer*1000);
  }

  this.stopTimer = function(){// stop the loop, nice to make available via a button.
    if(this.timeout != null)
      window.clearInterval(this.timeout);
    this.timeout = null;
  }
}

