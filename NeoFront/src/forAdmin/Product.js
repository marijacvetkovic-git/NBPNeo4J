import axios from "axios";
import { useState, useEffect } from "react";
import {
  Divider,
  Space,
  Typography,
  Dropdown,
  Input,
  Button,
  Form,
  InputNumber,
  Menu,
  message,
} from "antd";
const { Title } = Typography;

const ProductAdmin = () => {
  useEffect(() => {
    getProducts();
  }, []);

  const [products, setProducts] = useState([]);
  const [productData, setProductData] = useState({});
  const onFinish = (values) => {
    var name = values.productName;
    var u = values.use;
    var s = values.summary;

    axios
      .put(
        "https://localhost:5001/api/Product/Update_Product/" +
          name +
          "/" +
          u +
          "/" +
          s
      )
      .then((res) => {
        if (res.status === 202) {
          message.error("product not found");
        } else {
          message.success("successfully updated this product");
        }
      })
      .catch((err) => console.log(err.message));
  };
  const getProducts = () => {
    axios
      .get("https://localhost:5001/api/Product/Get_Products/")
      .then((res) => {
        setProducts(res.data);
      })
      .catch((err) => console.log(err.message));
  };
  const getProduct = (productName) => {
    axios
      .get(
        "https://localhost:5001/api/Product/Get_ProductForUpdate/" + productName
      )
      .then((res) => {
        setProductData({
          prName: res.data.productName,
          prUse: res.data.use,
          prSummary: res.data.summary,
        });
      })
      .catch((err) => console.log(err.message));
  };
  const onFinishAP = (values) => {
    var pn = values.productName;
    var pu = values.use;
    var ps = values.summary;
    const prod = {
      productName: pn,
      use: pu,
      summary: ps,
    };
    axios
      .post("https://localhost:5001/api/Product/Create_Product/", prod)
      .then((res) => {
        if (res.status != 202) {
          message.success("successfully created a product!");
        } else {
          message.error("product with that name already exists!");
        }
      })
      .catch((err) => console.log(err.message));
  };
  const SelectProduct = (productName) => {
    message.info("you selected product named " + productName + " for update");
    //console.log(productName);

    getProduct(productName);
  };
  const delteProduct = (e) => {
    var pr = e.prodName;
    axios
      .delete("https://localhost:5001/api/Product/Delete_Product/" + pr)
      .then((res) => {
        if (res.status === 202) message.error("product not found");
        else message.success("successfully deleted");
      })
      .catch((err) => console.log(err.message));
  };
  return (
    <>
      <Title level={3}>Product</Title>
      <Divider orientation="left">Add product</Divider>
      <Form
        name="basic"
        style={{
          maxWidth: 700,
        }}
        onFinish={onFinishAP}
      >
        <Form.Item
          label="Product name"
          name="productName"
          rules={[
            {
              required: true,
              message: "Please input product's name!",
            },
          ]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Product use"
          name="use"
          rules={[
            {
              required: true,
              message: "Please input product's use!",
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Product summary"
          name="summary"
          rules={[
            {
              required: true,
              message: "Please input product's summary!",
            },
          ]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          wrapperCol={{
            offset: 8,
            span: 16,
          }}
        >
          <Button type="primary" htmlType="submit">
            Create product
          </Button>
        </Form.Item>
      </Form>
      <Divider orientation="left">Update product</Divider>
      <Space size="large" align="start">
        <Form
          name="basic"
          style={{
            maxWidth: 700,
          }}
          onFinish={onFinish}
        >
          <Form.Item label="Product name" name="productName">
            <Input defaultValue={productData.prName} />
          </Form.Item>

          <Form.Item label="Product use" name="use">
            <Input defaultValue={productData.prUse} />
          </Form.Item>
          <Form.Item label="Product summary" name="summary">
            <Input defaultValue={productData.prSummary} />
          </Form.Item>

          <Form.Item
            wrapperCol={{
              offset: 8,
              span: 16,
            }}
          >
            <Button type="primary" htmlType="submit">
              Update product
            </Button>
          </Form.Item>
        </Form>
      </Space>
      <Divider orientation="left">Delete product</Divider>
      <div
        style={{
          display: "flex",
          justifyContent: "inline-block",
          paddingLeft: "50px",
        }}
      >
        <Space size="large">
          <Form
            name="basic"
            style={{
              maxWidth: 700,
            }}
            onFinish={delteProduct}
          >
            <Form.Item label="Product name" name="prodName">
              <Input />
            </Form.Item>

            <Form.Item
              wrapperCol={{
                offset: 8,
                span: 16,
              }}
            >
              <Button type="primary" htmlType="submit">
                Delete product
              </Button>
            </Form.Item>
          </Form>
        </Space>
      </div>
    </>
  );
};
export default ProductAdmin;
