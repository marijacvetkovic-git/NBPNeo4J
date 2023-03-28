import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Space, Checkbox, Form, Input } from "antd";
import axios from "axios";


const Login = () => {

  const onFinish = (values) => {
    console.log("Received values of form: ", values);
    var u = values.username;
    var p = values.password;
    // const user = {
    //   username: u,
    //   password: p,
    // };
    // const config = {
    //   headers: {
    //     "Access-Control-Allow-Origin": "*",
    //     "Content-Type": "application/json",
    //   },
    // };
    axios
      .get("https://localhost:5001/api/User/LogIn_User/" + u + "/" + p)
      .then((res) => {
        if (res.data.token != undefined) {
          localStorage.setItem("token", res.data.token);
          //window.location.reload();
        }
        else console.log("nema sifra bato");
        console.log(res.data, "Success");
        window.location = "/home";
      })
      .catch((err) => console.log(err.message));
  };
  return (
    <div
      className="login-wrap"
      style={{
        display: "flex",
        margin: "10% auto",
        justifyContent: "center",
        width: "200px",
        height: "250px",
      }}
    >
      <Form
        name="normal_login"
        className="login-form"
        initialValues={{
          remember: true,
        }}
        onFinish={onFinish}
      >
        <Form.Item
          id="username"
          name="username"
          rules={[
            {
              required: true,
              message: "Please input your Username!",
            },
          ]}
        >
          <Input
            prefix={<UserOutlined className="site-form-item-icon" />}
            placeholder="Username"
          />
        </Form.Item>
        <Form.Item
          id="password"
          name="password"
          rules={[
            {
              required: true,
              message: "Please input your Password!",
            },
          ]}
        >
          <Input
            prefix={<LockOutlined className="site-form-item-icon" />}
            type="password"
            placeholder="Password"
          />
        </Form.Item>
        <Form.Item>
          <Button
            type="primary"
            htmlType="submit"
            className="login-form-button"
          >
            Log in
          </Button>
          Or <a href="/register">register</a>
        </Form.Item>
      </Form>
    </div>
  );
};
export default Login;
