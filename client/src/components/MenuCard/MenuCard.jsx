import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Button from 'react-bootstrap/Button';
import Card from 'react-bootstrap/Card';
import './MenuCard.css';

class MenuCard extends Component{

  render(){
    return(
      <Card>
        <div>
          <i className={this.props.image}/>
        </div>
        <Card.Body className="cardBody">
          <Card.Title>{this.props.title}</Card.Title>
          <Card.Text>{this.props.text}</Card.Text>
          <Button onClick={() => {this.props.onClick()}} className="cardButton">{"Go to"}</Button>
        </Card.Body>
      </Card>
    );
  };
};

Card.propTypes = {
  image: PropTypes.string,
  title: PropTypes.string,
  text: PropTypes.string,
  data: PropTypes.array,
  onClick: PropTypes.func
}

export default MenuCard;
