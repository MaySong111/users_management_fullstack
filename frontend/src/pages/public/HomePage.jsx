import { useNavigate } from "react-router-dom";
import { getToken } from "../../http";
import { PATH_DASHBOARD, PATH_PUBLIC } from "../../Constant";
import { useEffect } from "react";

export default function HomePage() {
  const redirect = useNavigate();
  const token = getToken();

  useEffect(() => {
    if (token) {
      redirect(PATH_DASHBOARD.dashboard);
    }
  }, [token]);

  return (
    <div>
      <h1>Welcome,HomePage</h1>
      <button onClick={() => redirect(PATH_PUBLIC.login)}>Login</button>
      <button onClick={() => redirect(PATH_PUBLIC.register)}>Register</button>
    </div>
  );
}
