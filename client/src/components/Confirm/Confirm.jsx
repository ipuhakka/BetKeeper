import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Confirm extends Component{

  render(){

    const { props } = this;

    if (props.visible){

      return (
          <Alert bsStyle="danger">
            <h4>
              { props.headerText !== undefined ?
               props.headerText : 'Confirm action'}
            </h4>
            <p>
              <Button onClick={props.cancelAction} bsStyle="danger">{props.cancelText !== undefined ?
                props.cancelText : "Cancel"}
              </Button>
              <Button onClick={props.confirmAction} bsStyle="success">{props.confirmText !== undefined ?
                props.confirmText : "Confirm"}
              </Button>
            </p>
          </Alert>
        );
    }

    return (<div/>);
  }
}

Confirm.propTypes = {
  visible: PropTypes.bool.isRequired,
  confirmAction: PropTypes.func.isRequired,
  cancelAction: PropTypes.func.isRequired,
  confirmText: PropTypes.string,
  cancelText: PropTypes.string,
  headerText: PropTypes.string
};

export default Confirm;
