import React, { useState, useEffect } from "react";
import { Input, Card, Spin, Row, Col, Divider, Button } from "antd";
import { Link } from "react-router-dom";
import { CloseCircleOutlined } from "@ant-design/icons";
import axios from "axios";
import { getUsername } from "./utils";
import { Typography } from "antd";
const { Title } = Typography;

const Favourites = () => {
  const [liked, setLiked] = useState([]);
  const [others, setOthers] = useState([]);
  const [brands, setBrands] = useState([]);
  const [loading, setLoading] = useState(true);
  const username = getUsername();

  useEffect(() => {
    axios
      .get("https://localhost:5001/api/User/LikeList_User/" + username)
      .then((res) => {
        console.log(res.data, "Liked products");
        setLiked(res.data);
        setLoading(false);
      })
      .catch((err) => {
        console.log(err.message);
        setLoading(false);
      });
    axios
      .get(
        "https://localhost:5001/api/Filter/Recommend_ProductsBySimilarity/" +
          username
      )
      .then((res) => {
        console.log(res.data, "Liked products similarity");
        setOthers(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });

    axios
      .get(
        "https://localhost:5001/api/Filter/Recommend_ProductsByBrand/" +
          username
      )
      .then((res) => {
        console.log(res.data, "Liked products brand");
        setBrands(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
  }, []);

  const handleDislike = (productName) => {
    axios
      .delete(
        "https://localhost:5001/api/Relationships/Delete_Assign_Liked/" +
          username +
          "/" +
          productName
      )
      .then((res) => {
        console.log("Disliked");

        window.location.reload();
      })
      .catch((err) => {
        console.log(err.message);
      });
  };
  return (
    <div>
      <Title level={2}>Your liked products:</Title>
      <Row gutter={[16, 16]}>
        {loading ? (
          <Spin />
        ) : (
          liked.map((p) => (
            <Col span={6}>
              <Card
                title={
                  <div
                    style={{ display: "flex", justifyContent: "space-between" }}
                  >
                    <Link to={`/product/${p.product.productName}`}>
                      {p.product.productName}
                    </Link>
                    <Button
                      type="text"
                      onClick={() => handleDislike(p.product.productName)}
                      icon={<CloseCircleOutlined />}
                    />
                  </div>
                }
                bordered={false}
                style={{ width: 300 }}
              >
                <p>Use : {p.product.use}</p>
                <p>Summary : {p.product.summary}</p>
              </Card>
            </Col>
          ))
        )}
      </Row>
      <Divider orientation="left" orientationMargin="0">
        Others also liked:
      </Divider>
      <Row gutter={[16, 16]}>
        {others.map((p) => (
          <Col span={6}>
            <Card
              title={
                <div
                  style={{ display: "flex", justifyContent: "space-between" }}
                >
                  <Link to={`/product/${p.productName}`}>{p.productName}</Link>
                </div>
              }
              bordered={false}
              style={{ width: 300 }}
            >
              <p>Use : {p.use}</p>
              <p>Summary : {p.summary}</p>
            </Card>
          </Col>
        ))}
      </Row>
      <Divider orientation="left" orientationMargin="0">
        Best products of your favorite brand:
      </Divider>
      <Row gutter={[16, 16]}>
        {brands.map((p) => (
          <Col span={6}>
            <Card
              title={
                <div
                  style={{ display: "flex", justifyContent: "space-between" }}
                >
                  <Link to={`/product/${p.product.productName}`}>
                    {p.product.productName}
                  </Link>
                </div>
              }
              bordered={false}
              style={{ width: 300 }}
            >
              <p>Use : {p.product.use}</p>
              <p>Summary : {p.product.summary}</p>
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};
export default Favourites;
