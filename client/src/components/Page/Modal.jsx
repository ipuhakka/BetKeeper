import React, { Component } from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import * as pageActions from '../../actions/pageActions';
import Input from './Input';

/** Action modal. */
class Modal extends Component 
{
    constructor(props)
    {
      super(props);

      this.state = {
        actionResponseValues: {},
        hasInvalidInputs: false
      }

      this.onChangeInputValue = this.onChangeInputValue.bind(this);
    }

    onChangeInputValue(key, newValue)
    {
      const actionResponse = {...this.state.actionResponseValues};

      actionResponse[key] = newValue;

      this.setState({
        actionResponseValues: actionResponse,
        hasInvalidInputs: false
      });
    }

    renderFields()
    {
      const { actionFields } = this.props;

      return _.map(actionFields, field => {
        return <Input 
          onChange={this.onChangeInputValue}
          onError={() => 
            {
              this.setState({
                hasInvalidInputs: true
              });
            }}
          type={field.fieldType} 
          label={field.label}
          fieldKey={field.key}
          key={field.key} />
      });
    }

    render()
    {
        const { props, state } = this;

        return <RBModal show={props.show} onHide={props.onClose}>
          <RBModal.Header closeButton>
            <RBModal.Title>{props.title}</RBModal.Title>
          </RBModal.Header>
        
          <RBModal.Body>
            <div>
              {this.renderFields()}
            </div>
          </RBModal.Body>
        
          <RBModal.Footer>
            <Button 
              variant="outline-secondary" 
              onClick={props.onClose}>Close</Button>
            <Button 
              variant="outline-primary"
              disabled={state.hasInvalidInputs}
              onClick={() => 
              {
                pageActions.callModalAction(props.page, props.action, state.actionResponseValues);
              }}>Ok</Button>
          </RBModal.Footer>
        </RBModal>;
    }
};

Modal.defaultProps = {
  action: '',
  actionFields: [],
  title: ''
};

Modal.propTypes = {
  onClose: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  page: PropTypes.string.isRequired,
  action: PropTypes.string.isRequired,
  actionFields: PropTypes.arrayOf(
    PropTypes.shape({
      key: PropTypes.string.isRequired,
      fieldType: PropTypes.string.isRequired,
      label: PropTypes.string.isRequired
  })),
  title: PropTypes.string.isRequired
};

export default Modal;