/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the Source EULA. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
'use strict';

import * as azdata from 'azdata';
import * as vscode from 'vscode';

let toggleOn: boolean = false;
let statusView = vscode.window.createStatusBarItem(vscode.StatusBarAlignment.Right);
statusView.text = 'Query Plan watcher on';

export function activate(context: vscode.ExtensionContext) {
	vscode.commands.registerCommand('queryplan.ToggleProcessPlan', () => {
		toggleOn = !toggleOn;
		if (toggleOn) {
			statusView.show();
		} else {
			statusView.hide();
		}
	});

	azdata.queryeditor.registerQueryEventListener({
		onQueryEvent(type: azdata.queryeditor.QueryEvent, document: azdata.queryeditor.QueryDocument, args: any) {
			if (type === 'executionPlan') {
				if (toggleOn) {

					let tab3 = azdata.window.createTab('Webview Test 1');
					tab3.registerContent(async view => {

						let webview = view.modelBuilder.webView().component();
				
						let flexModel = view.modelBuilder.flexContainer().component();						
						flexModel.addItem(webview, { flex: '0' });
						flexModel.setLayout({
							flexFlow: 'column',
							alignItems: 'stretch',
							height: '100%'
						});

						await view.initializeModel(flexModel);

						setTimeout(() => {
							if (webview) {
								webview.html = "<html><body>Content there</body></html>";
							}
						}, 2000);
					});

					document.createQueryTab(tab3);

					/*


		let webview = view.modelBuilder.webView()
			.component();

		let flexModel = view.modelBuilder.flexContainer().component();
		flexModel.addItem(toolbarModel, { flex: '0' });
		flexModel.addItem(webview, { flex: '1' });
		flexModel.setLayout({
			flexFlow: 'column',
			alignItems: 'stretch',
			height: '100%'
		});

		let templateValues = { url: 'http://whoisactive.com/docs/' };
		// Utils.renderTemplateHtml(path.join(__dirname, '..'), 'templateTab.html', templateValues)
		// 	.then(html => {
		// 		webview.html = html;
		// 	});

		await view.initializeModel(flexModel);
					*/

					let tab2 = azdata.window.createTab('Query Watcher 2');
					let webview1 = undefined;
					tab2.registerContent(async view => {
						let webview1 = view.modelBuilder.webView()
							.withProperties({
								html: "<h1>Here it is</h1>"
							}).component();

						webview1.onMessage((params) => {
							webview1.html = '<h2>Updated Content</h2>';
						});

						let divModel = view.modelBuilder.divContainer()
							.withItems([webview1])
							.withLayout({ width: '100%', height: '100%' })
							.component();

						await view.initializeModel(divModel);


						let times: number = 1;
						setTimeout(() => {
							if (webview1) {
								// webview1.html = '<h2>Updated Content ' + times + '</h2>';
								webview1.updateProperties(
									{
										html: "<h1>Here it is timer</h1>"

									});
							}
						}, 5000);
					});



					document.createQueryTab(tab2);

					let tab = azdata.window.createTab('Query Watcher');
					tab.registerContent(async view => {
						let fileNameTextBox = view.modelBuilder.inputBox().component();
						let xmlTextBox = view.modelBuilder.inputBox().component();

						let formModel = view.modelBuilder.formContainer()
							.withFormItems([{
								component: fileNameTextBox,
								title: 'File name'
							}, {
								component: xmlTextBox,
								title: 'Plan XML'
							}]).withLayout({ width: '100%' }).component();

						await view.initializeModel(formModel);

						fileNameTextBox.value = document.uri;
						xmlTextBox.value = <string>args;
					});

					document.createQueryTab(tab);
				}
			}
		}
	});
}

// private openEditorWithWebview2(): void {
// 	let editor = sqlops.workspace.createModelViewEditor('Editor webview2', { retainContextWhenHidden: true });
// 	editor.registerContent(async view => {

		// let inputBox = view.modelBuilder.inputBox().component();
		// let dropdown = view.modelBuilder.dropDown()
		// 	.withProperties({
		// 		value: 'aa',
		// 		values: ['aa', 'bb', 'cc']
		// 	})
		// 	.component();
		// let runIcon = path.join(__dirname, '..', 'media', 'start.svg');
		// let runButton = view.modelBuilder.button()
		// 	.withProperties({
		// 		label: 'Run',
		// 		iconPath: runIcon,
		// 		title: 'Run title'
		// 	}).component();

		// let monitorLightPath = vscode.Uri.file(path.join(__dirname, '..', 'media', 'monitor.svg'));
		// let monitorIcon = {
		// 	light: monitorLightPath,
		// 	dark: path.join(__dirname, '..', 'media', 'monitor_inverse.svg')
		// };

		// let monitorButton = view.modelBuilder.button()
		// 	.withProperties({
		// 		label: 'Monitor',
		// 		iconPath: monitorIcon,
		// 		title: 'Monitor title'
		// 	}).component();
		// let toolbarModel = view.modelBuilder.toolbarContainer()
		// 	.withToolbarItems([{
		// 		component: inputBox,
		// 		title: 'User name:'
		// 	}, {
		// 		component: dropdown,
		// 		title: 'favorite:'
		// 	}, {
		// 		component: runButton
		// 	}, {
		// 		component: monitorButton
		// 	}]).component();


	// 	let webview = view.modelBuilder.webView()
	// 		.component();

	// 	let flexModel = view.modelBuilder.flexContainer().component();
	// 	flexModel.addItem(toolbarModel, { flex: '0' });
	// 	flexModel.addItem(webview, { flex: '1' });
	// 	flexModel.setLayout({
	// 		flexFlow: 'column',
	// 		alignItems: 'stretch',
	// 		height: '100%'
	// 	});

	// 	let templateValues = { url: 'http://whoisactive.com/docs/' };
	// 	// Utils.renderTemplateHtml(path.join(__dirname, '..'), 'templateTab.html', templateValues)
	// 	// 	.then(html => {
	// 	// 		webview.html = html;
	// 	// 	});

	// 	await view.initializeModel(flexModel);
	// });
	//editor.openEditor();
// }

// this method is called when your extension is deactivated
export function deactivate(): void {
}

export function escape(html: string): string {
	return html.replace(/[<|>|&|"]/g, function (match) {
		switch (match) {
			case '<': return '&lt;';
			case '>': return '&gt;';
			case '&': return '&amp;';
			case '"': return '&quot;';
			case '\'': return '&#39';
			default: return match;
		}
	});
}
