import React, { Component } from 'react';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import * as pageActions from '../../actions/pageActions';
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Confirm from '../../components/Confirm/Confirm';
import Button from '../../components/Page/Button';
import Field from '../../components/Page/Field';
import Modal from '../../components/Page/Modal';
import Table from '../../components/Page/Table';
import Info from '../../components/Info/Info.jsx';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            actionModalOpen: false,
            actionModalProps: {},
            data: {},
            confirm: {
                show: false,
                action: '',
                actionDataKeys: [],
                header: ''
            }
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
        this.clickNavigationButton = this.clickNavigationButton.bind(this);
        this.clickPageAction = this.clickPageAction.bind(this);
        this.onDataChange = this.onDataChange.bind(this);
        this.executePageAction = this.executePageAction.bind(this);
        this.executePageActionFromConfirm = this.executePageActionFromConfirm.bind(this);
    }

    onDataChange(key, newValue)
    {
        this.setState({
            data: {
                ...this.state.data,
                [key]: newValue
            }
        });
    }

    /**
     * Executes a page action after confirm.
     */
    executePageActionFromConfirm()
    {
        const {action, actionDataKeys } = this.state.confirm;

        this.executePageAction(action, actionDataKeys)
    }

    /**
     * Executes a page action.
     * @param {string} action 
     * @param {Array} actionDataKeys 
     */
    executePageAction(action, actionDataKeys)
    {
        const { state, props } = this;

        const parameters = {};

        _.forEach(actionDataKeys, dataKey => 
        {
            if (!_.isNil(state.data[dataKey]))
            {
                parameters[dataKey] = state.data[dataKey];
            }
            // parameter in page data
            else 
            {
                parameters[dataKey] = props.data[dataKey];
            }
        });

        const pathname = window.location.pathname;
        // Parse page name from path
        const pageKey = pathname.split('page/')[1];

        pageActions.callAction(pageKey, action, parameters);
    }

    /** Handles page action button click. Either opens a confirm dialog
     * or calls action directly.
     */
    clickPageAction(action, actionDataKeys, requireConfirm, confirmHeader, style)
    {
        if (requireConfirm)
        {
            this.setState({
                confirm: {
                    show: true,
                    action: action,
                    actionDataKeys: actionDataKeys,
                    header: confirmHeader,
                    variant: style
                }
            });
        }
        else 
        {
            this.executePageAction(action, actionDataKeys);
        }
    }

    clickModalAction(action, actionFields, title, requireConfirm, style)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                action,
                actionFields,
                title,
                requireConfirm,
                confirmVariant: style
            }
        });
    }

    clickNavigationButton(navigateTo)
    {
        this.props.history.push(navigateTo);
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        if (button.buttonType === 'PageAction')
        {
            return this.clickPageAction;
        }

        if (button.buttonType === 'Navigation')
        {
            return this.clickNavigationButton;
        }

        return null;
    }

    renderComponents(components, classname)
    {
        return <div key={classname} className={classname}>
            {_.map(components, (component, i) => 
            {
                switch (component.componentType)
                {
                    case 'Container':
                        return this.renderComponents(component.children, 'container-div');

                    case 'Button':
                        return <Button 
                            key={`button-${component.action}`}
                            onClick={this.getButtonClick(component)} 
                            {...component} />;

                    case 'Field':
                        return <Field 
                            onChange={this.onDataChange}
                            key={`field-${component.key}`} 
                            type={component.fieldType} 
                            fieldKey={component.key}
                            initialValue={_.get(this.props.data, component.dataKey, '')} 
                            {...component} />;

                    case 'Table':
                        return <Table onRowClick={this.clickNavigationButton} key={`itemlist-${i}`} {...component} />;

                    default:
                        throw new Error(`Component type ${component.componentType} not implemented`);
                }
            })}
        </div>;
    }
    
    render()
    {
        const { state, props } = this;

        const pathname = window.location.pathname;
        // Parse page name from path and convert first char to upper
        let pageKey = pathname
            .substring(pathname.lastIndexOf('/') + 1);

        pageKey = pageKey
            .charAt(0)
            .toUpperCase() + pageKey.slice(1);

        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                page={pageKey}
                show={state.actionModalOpen} 
                {...state.actionModalProps}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disableValue={pageKey}></Menu>
            <Confirm 
                visible={state.confirm.show}
                headerText={state.confirm.header}
                cancelAction={() => 
                {
                    this.setState({
                        confirm: {
                            show: false,
                            action: '',
                            actionDataKeys: [],
                            header: ''
                        }
                    });
                }}
                confirmAction={this.executePageActionFromConfirm}
                variant={state.confirm.variant}/>
            <Info alertState={state.alertState} alertText={state.alertText} dismiss={this.dismissAlert}></Info>
            {this.renderComponents(props.components, 'page-content')}
        </div>;
    }
};

export default withRouter(Page);