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

const BrandAdmin = () => {
  useEffect(() => {
    getProducts();
  }, []);
  const updateBrand = (values) => {
    var bn = values.UpdateBrandName;
    var bf = values.UpdateBrandFounder;
    var bs = values.updateBrandSummary;
    axios
      .put(
        "https://localhost:5001/api/Brand/Update_Brand/" +
          bn +
          "/" +
          bf +
          "/" +
          bs
      )
      .then((res) => {
        if (res.status === 202) {
          message.error("brand not found");
        } else {
          message.success("successfully updated this brand");
        }
      })
      .catch((err) => console.log(err.message));
  };
  const onFinishAB = (values) => {
    var bn = values.name;
    var bf = values.founder;
    var bs = values.summary;
    const brand = {
      name: bn,
      founder: bf,
      summary: bs,
    };
    axios
      .post("https://localhost:5001/api/Brand/Create_Brand/", brand)
      .then((res) => {
        if (res.status != 202) {
          message.success("successfully created a brand!");
        } else {
          message.error("brand with that name already exists!");
        }
      })
      .catch((err) => console.log(err.message));
  };
  const [products, setProducts] = useState([]);
  const [productData, setProductData] = useState();

  const getProducts = () => {
    axios
      .get("https://localhost:5001/api/Product/Get_Products/")
      .then((res) => {
        setProducts(res.data);
      })
      .catch((err) => console.log(err.message));
  };

  const deleteBrand = (e) => {
    var pr = e.brName;
    axios
      .delete("https://localhost:5001/api/Brand/Delete_Brand/" + pr)
      .then((res) => {
        if (res.status === 202) message.error("brand not found");
        else message.success("successfully deleted");
      })
      .catch((err) => console.log(err.message));
  };
  return (
    <>
      <Title level={3}>Brand</Title>
      <Divider orientation="left">Create brand</Divider>
      <Form
        name="basic"
        style={{
          maxWidth: 700,
        }}
        onFinish={onFinishAB}
      >
        <Form.Item
          label="Brand name"
          name="name"
          rules={[
            {
              required: true,
              message: "Please input brand's name!",
            },
          ]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Brand founder"
          name="founder"
          rules={[
            {
              required: true,
              message: "Please input brand's founder!",
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Brand summary"
          name="summary"
          rules={[
            {
              required: true,
              message: "Please input brand's summary!",
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
            Create brand
          </Button>
        </Form.Item>
      </Form>
      <Divider orientation="left">Update brand</Divider>
      <Space size="large" align="start">
        <Form
          name="basic"
          style={{
            maxWidth: 700,
          }}
          onFinish={updateBrand}
        >
          <Form.Item
            label="Brand name"
            name="UpdateBrandName"
            rules={[
              {
                required: true,
                message: "Please input brand's name!",
              },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Brand founder"
            name="UpdateBrandFounder"
            rules={[
              {
                required: true,
                message: "Please input brand's founder!",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Brand summary"
            name="updateBrandSummary"
            rules={[
              {
                required: true,
                message: "Please input brand's summary!",
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
              Update brand
            </Button>
          </Form.Item>
        </Form>
      </Space>
      <Divider orientation="left">Delete brand</Divider>
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
            onFinish={deleteBrand}
          >
            <Form.Item
              label="Brand name"
              name="brName"
              rules={[
                {
                  required: true,
                  message: "Please input brand's name!",
                },
              ]}
            >
              <Input />
            </Form.Item>
            <Button type="primary" htmlType="onClick">
              Delete brand
            </Button>
          </Form>
        </Space>
      </div>
    </>
  );
};
export default BrandAdmin;
