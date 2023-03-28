import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import {
  Input,
  Card,
  Row,
  Col,
  Checkbox,
  Button,
  Form,
  Collapse,
  Space,
} from "antd";
import axios from "axios";
import { HeartOutlined } from "@ant-design/icons";
import { getUsername } from "./utils";
const { Search } = Input;
const Home = () => {
  const username = getUsername();
  const [products, setProducts] = useState([]);
  const [brands, setBrands] = useState([]);
  const [ing, setIngredients] = useState([]);
  const [ptype, setPType] = useState([]);
  const [stype, setSType] = useState([]);
  const [selectedBrands, setSelectedBrands] = useState([]);
  const [selectedIng, setSelectedIng] = useState([]);
  const [selectedPT, setSelectedPT] = useState([]);
  const [selectedST, setSelectedST] = useState([]);
  const [, setTick] = useState(0);
  const [notfound, setNotFound] = useState(false);
  /////////////////////////////////////////////////////
  const onSearch = (value) => {
    axios
      .get("https://localhost:5001/api/User/SearchEngine_Products/" + value)
      .then((res) => {
        if (res.status === 202) {
          setNotFound(true);
        } else {
          setNotFound(false);
          setProducts(res.data);
        }
      })
      .catch((err) => console.log(err.message));
  };
  const { Panel } = Collapse;
  const forceUpdate = () => {
    setTick((tick) => tick + 1);
  };

  useEffect(() => {
    if (localStorage.getItem("token") == null) {
      axios
        .get("https://localhost:5001/api/Product/Get_Products")
        .then((res) => {
          console.log(res.data, "Izbacio sve");
          setProducts(res.data);
        })
        .catch((err) => console.log(err.message));
    } else {
      axios
        .get("https://localhost:5001/api/Product/GetNotLiked/" + username)
        .then((res) => {
          console.log(res.data, "Izbacio sve od usera");
          setProducts(res.data);
        })
        .catch((err) => console.log(err.message));
    }

    axios
      .get("https://localhost:5001/api/User/LikeList_User/" + username)
      .then((res) => {
        console.log(res.data, "Liked products");
        setProducts((prevProducts) => {
          return prevProducts.map((p) => {
            if (res.data.includes(p.productName)) {
              return { ...p, liked: true };
            } else {
              return p;
            }
          });
        });
      })
      .catch((err) => {
        console.log(err.message);
      });
    //brands
    axios
      .get("https://localhost:5001/api/Brand/Get_Brands")
      .then((res) => {
        console.log(res.data);
        setBrands(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
    //ingredient
    axios
      .get("https://localhost:5001/api/Ingredient/Get_Ingredients")
      .then((res) => {
        console.log(res.data);
        setIngredients(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
    //productType
    axios
      .get("https://localhost:5001/api/ProductType/Get_ProductType")
      .then((res) => {
        console.log(res.data);
        setPType(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
    //skinType
    axios
      .get("https://localhost:5001/api/SkinType/Get_SkinType")
      .then((res) => {
        console.log(res.data);
        setSType(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });

    forceUpdate();
  }, []);

  const handleLike = (productName) => {
    axios
      .post(
        "https://localhost:5001/api/Relationships/Assign_Likes/" +
          username +
          "/" +
          productName
      )
      .then((res) => {
        console.log("Liked");

        window.location.reload();
      })
      .catch((err) => {
        console.log(err.message);
      });
  };
  /////////////////////////////////////////////////
  const onChange = (e) => {
    var value = e.target.value;
    //console.log(e.target.value);
    setSelectedBrands((p) => {
      if (e.target.checked) {
        return [...p, value];
      } else {
        return p.filter((v) => v !== value);
      }
    });
  };
  const onChangeIng = (e) => {
    var value = e.target.value;
    //console.log(e.target.value);
    setSelectedIng((p) => {
      if (e.target.checked) {
        return [...p, value];
      } else {
        return p.filter((v) => v !== value);
      }
    });
  };
  const onChangePT = (e) => {
    var value = e.target.value;
    //console.log(e.target.value);
    setSelectedPT((p) => {
      if (e.target.checked) {
        return [...p, value];
      } else {
        return p.filter((v) => v !== value);
      }
    });
  };
  const onChangeST = (e) => {
    var value = e.target.value;
    //console.log(e.target.value);
    setSelectedST((p) => {
      if (e.target.checked) {
        return [...p, value];
      } else {
        return p.filter((v) => v !== value);
      }
    });
  };
  const update = (slB, slI, slPT, slST) => {
    const bodyUp = {
      brands: slB,
      skinTypes: slST,
      productTypes: slPT,
      indredients: slI,
    };

    console.log(bodyUp);
    axios
      .put("https://localhost:5001/api/Filter/Filter/", bodyUp)
      .then((res) => {
        console.log(res.data);
        setProducts(res.data);
      })
      .catch((err) => {
        console.log(err.message);
      });
  };
  /////////////////////////////////////////////////////
  return (
    <div>
      <Space
        direction="vertical"
        size="middle"
        style={{
          display: "flex",
        }}
      >
        <Search
          placeholder="input search text"
          onSearch={onSearch}
          enterButton
        />

        <Form>
          <Collapse>
            <Panel header="Brand">
              <Form.Item>
                {brands.map((p) => {
                  return (
                    <Checkbox name="brName" value={p.name} onChange={onChange}>
                      {p.name}
                    </Checkbox>
                  );
                })}
              </Form.Item>
            </Panel>

            <Panel header="Ingredients">
              <Form.Item>
                {ing.map((i) => {
                  return (
                    <Checkbox
                      name="ingName"
                      value={i.name}
                      onChange={onChangeIng}
                    >
                      {i.name}
                    </Checkbox>
                  );
                })}
              </Form.Item>
            </Panel>
            <Panel header="Product type">
              <Form.Item>
                {ptype.map((pt) => {
                  return (
                    <Checkbox
                      name="prodType"
                      value={pt.name}
                      onChange={onChangePT}
                    >
                      {pt.name}
                    </Checkbox>
                  );
                })}
              </Form.Item>
            </Panel>
            <Panel header="Skin type">
              <Form.Item>
                {stype.map((st) => {
                  return (
                    <Checkbox
                      name="skinType"
                      value={st.sType}
                      onChange={onChangeST}
                    >
                      {st.sType}
                    </Checkbox>
                  );
                })}
              </Form.Item>
            </Panel>
          </Collapse>
          <div style={{ textAlign: "center" }}>
            <Form.Item>
              <Button
                type="primary"
                htmlType="onClick"
                className="login-form-button"
                style={{
                  width: "10rem",
                  marginTop: "1rem",
                }}
                onClick={() => {
                  update(selectedBrands, selectedIng, selectedPT, selectedST);
                }}
              >
                Find products
              </Button>
            </Form.Item>
          </div>
        </Form>
        {notfound === true ? (
          <p>no products matched the description :/</p>
        ) : (
          <></>
        )}
      </Space>
      <Row gutter={[16, 16]}>
        {products.map((p) => {
          return (
            <Col xs={28} sm={16} md={12} lg={10} xl={8}>
              <Card
                type="inner"
                title={
                  <div
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                    }}
                  >
                    <Link to={`/product/${p.productName}`}>
                      {p.productName}
                    </Link>
                  </div>
                }
                extra={
                  <Button
                    type="text"
                    onClick={() => handleLike(p.productName)}
                    icon={<HeartOutlined />}
                  />
                }
                bordered={false}
                style={{ width: "100%" }}
              >
                <p>Use : {p.use}</p>
                <p>Summary : {p.summary}</p>
              </Card>
            </Col>
          );
        })}
      </Row>
    </div>
  );
};
export default Home;
