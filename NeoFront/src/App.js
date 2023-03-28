import "./App.css";
import "antd/dist/reset.css";
import { getUsername } from "./utils";
import {
  LoginOutlined,
  CrownOutlined,
  HomeOutlined,
  HeartOutlined,
} from "@ant-design/icons";
import { Breadcrumb, Layout, Menu, theme } from "antd";
import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import Login from "./LoginPage";
import Register from "./RegisterPage";
import Home from "./HomePage";
import Favourites from "./FavouritesPage";
import Admin from "./AdminPage";
import MenuItem from "antd/es/menu/MenuItem";
import { useState, useEffect } from "react";
import Product from "./ProductPage";
const { Header, Content, Footer } = Layout;
function App() {
  const {
    token: { colorBgContainer },
  } = theme.useToken();

  const logout = () => {
    localStorage.removeItem("token");
    window.location = "/";
  };
  const username = getUsername();
  const [isAdmin, setIsAdmin] = useState(false);
  useEffect(() => {
    if (username === "admin") {
      setIsAdmin(true);
    }
  }, [username]);
  return (
    <Router>
      <Layout className="layout">
        <Header>
          <Menu theme="dark" mode="horizontal" style={{ display: "block" }}>
            <MenuItem
              key={"1"}
              icon={<HomeOutlined />}
              style={{ float: "left" }}
            >
              <Link to={"/home"}>Home</Link>
            </MenuItem>
            <MenuItem
              key={"2"}
              icon={<HeartOutlined />}
              style={{ float: "left" }}
            >
              <Link to={"/favourites"}>Favourites</Link>
            </MenuItem>
            {isAdmin ? (
              <MenuItem
                key={"3"}
                icon={<CrownOutlined />}
                style={{ float: "left" }}
              >
                <Link to={"/admin"}>Admin</Link>
              </MenuItem>
            ) : (
              <></>
            )}
            <Menu.Item
              key={"4"}
              icon={<LoginOutlined />}
              style={{ float: "right" }}
            >
              <Link to={"/register"}>Register/Login</Link>
            </Menu.Item>

            <Menu.Item
              key={"5"}
              icon={<LoginOutlined />}
              style={{ float: "right" }}
            >
              <Link onClick={logout}>Logout</Link>
            </Menu.Item>
          </Menu>
        </Header>
        <Content
          style={{
            padding: "50px 50px",
          }}
        >
          {/* part for breadcrumbs, look up layout page */}
          <div
            className="site-layout-content"
            style={{
              background: colorBgContainer,
              width: "100%",
            }}
          >
            {/* content of each page */}
            <Routes>
              <Route path="/" element={<Register />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route path="/home" element={<Home />} />
              <Route path="/favourites" element={<Favourites />} />
              <Route path="/admin" element={<Admin />} />
              
              {/* <Route path="/product" element={<Product />} /> */}
              <Route path="/product/:prodName" element={<Product />} />
            </Routes>
          </div>
        </Content>
        <Footer
          style={{
            textAlign: "center",
          }}
        >
          noFil[lt]er ©2023 Created by GOFC
        </Footer>
      </Layout>
    </Router>
  );
}

export default App;
