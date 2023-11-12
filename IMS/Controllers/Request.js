/**
 * 返回一个AJAX请求的响应结果。
 *
 * @param {Object} settings - AJAX请求的设置参数。
 * @return {Object} AJAX请求的响应结果，如果请求失败则返回null。
 */
function getAjaxResponse(settings) {
    var result
    $.ajax(settings).done(function (response) {
        result =  response;
    }).fail(function (jqXHR, textStatus, errorThrown) {
        //在fail方法中处理错误情况
        console.error("Ajax request failed: " + textStatus + ", " + errorThrown); //打印错误信息
        result = null; //给变量赋空值
        alert("登录失败，请检查您的网络状态"); //弹出提示信息
    });
    return result;
}

/**
 * 用户登录函数
 *
 * @param {string} username - 用户名.
 * @param {string} password - 密码.
 * @return {Object} 返回获取到的数据，如果发送错误，返回空值
 */
async function login(username, password) {
    /*返回值示例{
    code: 0, 
    message: '登录成功！', 
    authenticationCode: 'BB8534194A8E32EBB2FD2B737C75BDE2FEA7C8A5D2D36DC9BF3C0F83F2711D7B', 
    uid: 42}*/
    const settings = {
        "url": "https://localhost:7018/user/login",
        "method": "POST",
        "timeout": 2000, // 超时时间设置为2秒
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "Username": "123",
            "Password": "123789422ABC"
        }),
        "async": false
    };
    return getAjaxResponse(settings);
}

/**
 * 注册一个新用户，使用给定的用户名、密码和电子邮件。
 *
 * @param {string} username - 新用户的用户名。
 * @param {string} password - 新用户的密码。
 * @param {string} email - 新用户的电子邮件。
 * @return {Object} 返回获取到的数据，如果发送错误，返回空值
 */
async function register(username, password, email) {
    var settings = {
        "url": "https://localhost:7018/user/register",
        "method": "POST",
        "timeout": 2000,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "Username": username,
            "Password": password,
            "Email": email
        }),
        "async": false
    };
    return  getAjaxResponse(settings);
}

/**
 * 向服务器发送确认请求，以确认用户注册。
 *
 * @param {string} uid - 用户ID。
 * @param {string} code - 确认码。
 * @return {Object} 返回获取到的数据，如果发送错误，返回空值
 */
function confirmRegister(uid,code) {
    var settings = {
        "url": "https://localhost:7018/user/confirm",
        "method": "POST",
        "timeout": 2000,
        "headers": {
            "Content-Type": "application/json"
        },
        "data": JSON.stringify({
            "Uid": uid,
            "CheckCode": code
        }),
    };

    return getAjaxResponse(settings);
}