import { Navigate, Outlet } from "react-router-dom";
import { getRoles, getToken } from "./http";
import { PATH_PUBLIC } from "./Constant";

export default function AuthGard({ requiredRole }) {
  const accessToken = getToken();
  const accessRole = getRoles(); // 取出来是字符串: '["user", "manager"]'
  console.log(accessRole);

  // 1. 先检查是否登录, 这个是最应该先检查的 : 没有 token → 用户根本没登录 → 直接重定向到 /login, 有 token → 才进入下一步角色/权限判断
  // 如果没有 token(未登录) -> 跳转到登录页 if token not exists--即没登陆--- redirect to login page
  // 当前行为：
  // 访问 http://localhost:3000/dashboard
  // 没有 token，显示 <LoginPage/>
  // 但 URL 还是 /dashboard ❌.   ---这我自己测试看起来都很奇怪,我当时还以为定义在App.jsx的path 和element不匹配呢,
  // if(!accessToken) {
  //   return <LoginPage/>       // URL 和页面内容不匹配
  // }
  // 那应该怎么做呢, 重定向
  // if (!accessToken) {
  //   return <Navigate to={PATH_PUBLIC.login} replace />;
  // }
  // // 那上面<LoginPage/>改成<Navigate后:比如前端localstorage没有token的时候,输入http://localhost:3000/dashboard--后会看到地址栏变成了http://localhost:3000/login且页面显示login页面
  // // URL 和页面内容一致---URL变成/login

  // // 2. 好有token之后,接下来第二步, 检查要请求的url--这个页面是否需要特定的角色:
  // // 逻辑检查:localStorage是否存角色 → 无则重定向 到未授权页
  // //                              -> 有的话, 那继续判断是否符合角色要求2.1
  // // 对于路由定义中App.jsx中 没有requiredRole 这个属性的, 那就是普通用户也能打开的,那就是不需要角色--这就是表示只要localstorage有token的就应该能显示
  // // 但是我下面这样写就是普通用户哪怕有token,那输入http://localhost:3000/dashboard 只会显示未授权, 这不对
  // if (requiredRole) {
  //   // 2.1 需要特定角色但用户角色不符() 如果前端localstorage中没有存储角色信息 -> 显示未授权页面)
  //   if (!accessRole) {
  //     // return <UnauthorizedPage/>. --更好的是用重定向
  //     return <Navigate to={PATH_PUBLIC.unauthorized} replace />;
  //   }

  //   // 有token,但是localstorage中没有存储角色信息
  //   return <Navigate to={PATH_PUBLIC.unauthorized} replace />;
  // }

  // 上面的思路很乱(所有没有 token 的访问都会跳登录”，包括访问未定义路径 → 404 被跳过),但是要看, 因为要明白到底是为什么乱的, 才能明白为什么下面的对
  // 第一: 输入没有的路径, 那显示不存在=---第二: 输入了需要 身份或者token的那就是显示 未授权 ----而不是跳转到login页面
  // 整个思路很乱, 很乱, 一定要好好梳理!!!!!!!!!!!!!
  if (!accessToken) {
    return <Navigate to={PATH_PUBLIC.unauthorized} replace />; //错了很多遍, 将近一下午才知道不应该是login
  }

  if (requiredRole) {
    if (!accessRole || accessRole !== requiredRole) {
      return <Navigate to={PATH_PUBLIC.unauthorized} replace />;
    }
  }
  return <Outlet />;
}
