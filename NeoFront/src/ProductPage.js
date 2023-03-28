import React, { useState, useEffect } from "react";
import axios from "axios";
import { Divider, Typography } from "antd";
const { Title } = Typography;

const Product = () => {
  const prodName = window.location.href.split("/")[4];
  const [pr, setProduct] = useState({});
  const [ing, setIngredients] = useState([]);
  const [st, setSkinTypes] = useState([]);
  const showProduct = () => {
    axios
      .get("https://localhost:5001/api/Product/Get_Product/" + prodName)
      .then((res) => {
        setProduct({
          productName: res.data.produktName,
          use: res.data.productUse,
          summary: res.data.productSummary,
          brand: res.data.brand,
          productType: res.data.prodType,
        });
      })
      .catch((err) => {
        console.log(err.message);
      });
  };
  const showIng = () => {
    axios
      .get(
        "https://localhost:5001/api/Product/Get_ProductIngredients/" + prodName
      )
      .then((res) => {
        setIngredients(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
  };

  const showSkinTypes = () => {
    axios
      .get(
        "https://localhost:5001/api/Product/Get_ProductSkinTypes/" + prodName
      )
      .then((res) => {
        setSkinTypes(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
  };
  useEffect(() => {
    showProduct();
    showIng();
    showSkinTypes();
  });

  return (
    <>
      <Title level={3}>{pr.productName}</Title>
      <Divider orientation="left">Name of product:</Divider>
      <p>{pr.productName}</p>
      <Divider orientation="left">Usage:</Divider>
      <p> {pr.use}</p>
      <Divider orientation="left">Summary: </Divider>
      <p> {pr.summary}</p>
      <Divider orientation="left">Brand: </Divider>
      <p>{pr.brand}</p>
      <Divider orientation="left">Product type: </Divider>
      <p>{pr.productType}</p>
      <Divider orientation="left">Product is intendent for: </Divider>
      {st.map((p) => {
        return <p>{p.sType}</p>;
      })}
      <Divider orientation="left">Product contains:</Divider>
      {ing.map((p) => {
        return <p>{p.name}</p>;
      })}
    </>
  );
};
export default Product;
