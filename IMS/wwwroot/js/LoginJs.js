let switchCtn = document.querySelector("#switch-cnt");
let switchC1 = document.querySelector("#switch-c1");
let switchC2 = document.querySelector("#switch-c2");
let switchCircle = document.querySelectorAll(".switch__circle");
let switchBtn = document.querySelectorAll(".switch-btn");
let aContainer = document.querySelector("#a-container");
let bContainer = document.querySelector("#b-container");
let allButtons = document.querySelectorAll(".submit");

let getButtons = (e) => e.preventDefault()

let changeForm = (e) => {

    switchCtn.classList.add("is-gx");
    setTimeout(function(){
        switchCtn.classList.remove("is-gx");
    }, 1500)

    switchCtn.classList.toggle("is-txr");
    switchCircle[0].classList.toggle("is-txr");
    switchCircle[1].classList.toggle("is-txr");

    switchC1.classList.toggle("is-hidden");
    switchC2.classList.toggle("is-hidden");
    aContainer.classList.toggle("is-txl");
    bContainer.classList.toggle("is-txl");
    bContainer.classList.toggle("is-z200");
}

let mainF = (e) => {
    for (var i = 0; i < allButtons.length; i++)
        allButtons[i].addEventListener("click", getButtons );
    for (var i = 0; i < switchBtn.length; i++)
        switchBtn[i].addEventListener("click", changeForm)
    
}

window.addEventListener("load", mainF);

var CommonHeader = new Headers();
CommonHeader.append("Content-Type", "application/json");
CommonHeader.append("Accept", "application/json");

function LoginPost() {
    let user = document.getElementById("LoginUsername").value;
    let pass = document.getElementById("LoginPassword").value;
    var requestOptions = {
        method: 'POST',
        headers: CommonHeader,
        body: JSON.stringify({
            "Username": user,
            "Password": pass
        }),
        redirect: 'follow'
    };

    fetch("https://localhost:7018/user/login", requestOptions)
        .then(response => response.json())
        .then(result => {
            console.log(result);
            if (result.status != 0){
                LoginFailed(result.message);
            }else{
                LoginSuccess();
            }
        })
        .catch(error => console.log('error', error));
}

// 修改message.js的设置
Qmsg.config({
    showClose:true,
    timeout: 2000
})


function LoginSuccess(){
    Qmsg.success("登录成功");
    // 两秒后切换界面
    setTimeout(2000,function (){
        window.location.href = "https://localhost:7018/";
    })
    Qmsg.clear();
}

function LoginFailed(msg){
    Qmsg.error("登录失败:" + msg);
}

function ConfirmEmailPost() {
    let user = document.getElementById("RegisterUsername").value;
    let pass = document.getElementById("RegisterPassword").value;
    let email = document.getElementById("RegisterEmail").value; 
    if (user == null || user.length === 0 ||user.length < 4 || user.length > 16) {
        Qmsg.error("用户名不合法");
        return;
    }
    if (pass == null || pass.length === 0 ||pass.length < 4 || pass.length > 16) {
        Qmsg.error("密码不合法");
        return;
    }
    var EmailReg = /^([a-zA-Z]|[0-9])(\w|-)+@[a-zA-Z0-9]+\.([a-zA-Z]{2,4})$/;
    if (!EmailReg.test(email)) {
        Qmsg.error("邮箱不合法");
        return;
    }
    console.log(JSON.stringify({
        "Username": user,
        "Password": pass,
        "Email": email
    }));
    var requestOptions = {
        method: 'POST',
        headers: CommonHeader,
        body: JSON.stringify({
            "Username": user,
            "Password": pass,
            "Email": email
        }),
        redirect: 'follow'
    };

    fetch("https://localhost:7018/user/Register", requestOptions)
        .then(response => response.json())
        .then(result => {
            console.log(result);
            console.log(result.status)
            if (result.status !== 0){
                ConfirmFailed(result.message);
            }else{
                PostSuccess();
                Uid = result.data;
                RegisterUsername = user;
                // 为了防止信息发送错误，应该在验证成功是锁住用户名和密码框
                document.getElementById("RegisterUsername").readOnly = true;
                document.getElementById("RegisterPassword").readOnly = true;
                document.getElementById("RegisterEmail").readOnly = true;
                console.log(Uid);
            }
        })
        .catch(error => console.log('error', error));
}

var Uid = -1; // 存储发来的数据
var RegisterUsername = "";

function RegisterPost(){
    let ConfirmCode = document.getElementById("ConfirmCode").value;
    if(ConfirmCode == null || ConfirmCode.length === 0){
        Qmsg.error("验证码不能为空");
        return;
    }
    
    var requestOptions = {
        method: 'POST',
        headers: CommonHeader,
        body: JSON.stringify({
            "Uid" : Uid,
            "CheckCode": ConfirmCode
        }),
        redirect: 'follow'
    };
    
    console.log(JSON.stringify({
        "Uid" : Uid,
        "CheckCode": ConfirmCode
    }));
    
    fetch("https://localhost:7018/user/confirm", requestOptions)
        .then(response => response.json())
        .then(result => {
            console.log(result);
            if (result.status !== 0){
                ConfirmFailed(result.message);
            }else{
                Qmsg.success("注册成功");
                setTimeout(function (){
                    Qmsg.closeAll();
                    document.getElementById("GoLogin").click();
                    document.getElementById("LoginUsername").value = RegisterUsername;
                },2000)
            }
        })
        .catch(error => console.log('error', error));
}

function PostSuccess(){
    Qmsg.success("验证邮件已经发送，请查收");
}

function ConfirmFailed(msg){
    Qmsg.error("错误:" + msg);
} 

document.getElementById("Confirm").addEventListener("click",function (e){
    ConfirmEmailPost();
    e.preventDefault();
})

document.getElementById("Register").addEventListener("click",function (e){
    RegisterPost();
    e.preventDefault();
})

document.getElementById("Login").addEventListener("click",function (e){
    LoginPost();
    e.preventDefault();
})

document.getElementById("ForgetPassword").addEventListener("click",function (e){
    Qmsg.info("私密马赛，这个功能还没实现哦！")
    e.preventDefault();
})