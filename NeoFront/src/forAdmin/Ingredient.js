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

const IngredientAdmin = () => {
  useEffect(() => {
    getProducts();
  }, []);
  const onFinish = (values) => {};
  const onFinishAI = (values) => {
    var ingn = values.name;
    var iu = values.usage;
    var ii = values.irritancy;
    const ing = {
      name: ingn,
      usage: iu,
      irritancy: ii,
    };
    axios
      .post("https://localhost:5001/api/Ingredient/Create_Ingredient/", ing)
      .then((res) => {
        if (res.status != 202) {
          message.success("successfully created an ingredient!");
        } else {
          message.error("ingredient with that name already exists!");
        }
      })
      .catch((err) => console.log(err.message));
  };

  const items = [
    {
      key: "1",
      label: (
        <a
          target="_blank"
          rel="noopener noreferrer"
          href="https://www.antgroup.com"
        >
          1st menu item
        </a>
      ),
    },
    {
      key: "2",
      label: (
        <a
          target="_blank"
          rel="noopener noreferrer"
          href="https://www.aliyun.com"
        >
          2nd menu item (disabled)
        </a>
      ),
    },
    {
      key: "3",
      label: (
        <a
          target="_blank"
          rel="noopener noreferrer"
          href="https://www.luohanacademy.com"
        >
          3rd menu item (disabled)
        </a>
      ),
      disabled: true,
    },
    {
      key: "4",
      danger: true,
      label: "a danger item",
    },
  ];

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
  const updateIng = (e) => {
    var iname = e.UpdateIngredientName;
    var usage = e.UpdateIngredientUsage;
    var irt = e.updateIngredientIrritancy;
    axios
      .put(
        "https://localhost:5001/api/Ingredient/Update_Ingredient/" +
          iname +
          "/" +
          usage +
          "/" +
          irt
      )
      .then((res) => {
        if (res.status === 202) {
          message.error("ingredient not found");
        } else {
          message.success("successfully updated this ingredient");
        }
      })
      .catch((err) => console.log(err.message));
  };
  // /api/Ingredient/Update_Ingredient/{name}/{usage}/{irritancy}
  const delteIngredient = (e) => {
    var pr = e.ingName;
    axios
      .delete("https://localhost:5001/api/Ingredient/Delete_Ingredient/" + pr)
      .then((res) => {
        if (res.status === 202) message.error("ingredient not found");
        else message.success("successfully deleted");
      })
      .catch((err) => console.log(err.message));
  };

  return (
    <>
      <Title level={3}>Ingredient</Title>
      <Divider orientation="left">Create ingredient</Divider>
      <Form
        name="basic"
        style={{
          maxWidth: 700,
        }}
        onFinish={onFinishAI}
      >
        <Form.Item
          label="Ingredient name"
          name="name"
          rules={[
            {
              required: true,
              message: "Please input ingredient's name!",
            },
          ]}
        >
          <Input />
        </Form.Item>

        <Form.Item
          label="Ingredient usage"
          name="usage"
          rules={[
            {
              required: true,
              message: "Please input ingredient's usage!",
            },
          ]}
        >
          <Input />
        </Form.Item>
        <Form.Item
          label="Ingredient irritancy"
          name="irritancy"
          rules={[
            {
              required: true,
              message: "Please input ingredient's irritancy!",
            },
          ]}
        >
          <InputNumber min={1} max={10} defaultValue={3} />
        </Form.Item>

        <Form.Item
          wrapperCol={{
            offset: 8,
            span: 16,
          }}
        >
          <Button type="primary" htmlType="submit">
            Create ingredient
          </Button>
        </Form.Item>
      </Form>
      <Divider orientation="left">Update ingredient</Divider>
      <Space size="large" align="start">
        <Form
          name="basic"
          style={{
            maxWidth: 700,
          }}
          onFinish={updateIng}
        >
          <Form.Item
            label="Ingredient name"
            name="UpdateIngredientName"
            rules={[
              {
                required: true,
                message: "Please input ingredient's name!",
              },
            ]}
          >
            <Input />
          </Form.Item>

          <Form.Item
            label="Ingredient usage"
            name="UpdateIngredientUsage"
            rules={[
              {
                required: true,
                message: "Please input ingredient's usage!",
              },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            label="Ingredient irritancy"
            name="updateIngredientIrritancy"
            rules={[
              {
                required: true,
                message: "Please input ingredient's irritancy!",
              },
            ]}
          >
            <InputNumber min={1} max={10} defaultValue={3} />
          </Form.Item>

          <Form.Item
            wrapperCol={{
              offset: 8,
              span: 16,
            }}
          >
            <Button type="primary" htmlType="submit">
              Update ingredient
            </Button>
          </Form.Item>
        </Form>
      </Space>
      <Divider orientation="left">Delete ingredient</Divider>
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
            onFinish={delteIngredient}
          >
            <Form.Item label="Ingredient name" name="ingName">
              <Input />
            </Form.Item>

            <Form.Item
              wrapperCol={{
                offset: 8,
                span: 16,
              }}
            >
              <Button type="primary" htmlType="submit">
                Delete ingredient
              </Button>
            </Form.Item>
          </Form>
        </Space>
      </div>
    </>
  );
};
export default IngredientAdmin;
